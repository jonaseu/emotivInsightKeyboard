using System;
using System.Net.Sockets;
using System.Text;
using Keyboard.Controllers;

namespace Keyboard.Business_Rules
{
    public class StateObject
    { 
        public Socket WorkSocket;
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder Sb = new StringBuilder();
    }


    class rulKeyboard
    {
        private System.Timers.Timer _blinkTimer;
        private System.Timers.Timer _checker;
        private readonly frmKeyboard _form;
        private int _currentLine;
        private int _currentColumn;
        private bool ShouldBlink;
        private bool blinkLine = true;
        
        //Emotiv Configs
        private Socket _sckEmoEngine = null;
        private string _ptoConnect = "";
        private int _clickMode = 0;
        private int _sensibility = 0;
        private int _timeInterval = 0;

        public rulKeyboard(frmKeyboard frm)
        {
            _form = frm;
        }
        
        public bool ConnectEmotiv(string host, int port, int interval)
        {
            _sckEmoEngine = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            StateObject state = new StateObject { WorkSocket = _sckEmoEngine };

            try
            {
                _sckEmoEngine.Connect(host, 1900);
                BeginAlternateLines(interval);
            }
            catch (SocketException socktEx)
            {
                Console.WriteLine(socktEx);
                return false;
            }
            _sckEmoEngine.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                ReceiveCallback, state);
            
            _checker = new System.Timers.Timer(1000);
            _checker.Elapsed += IsAlive;
            _checker.Start();
            return true;
        }

        private void IsAlive(object sender, EventArgs e)
        {
            bool answered = _sckEmoEngine.Poll(1000, SelectMode.SelectRead);
            bool isSomething = (_sckEmoEngine.Available == 0);
            if (answered && isSomething)
            {
                StopAlternateLines();
                _form.LostConnection();
            }
        }
        
        public void DisconnectEmotiv()
        {
            _sckEmoEngine.Disconnect(false);
            StopAlternateLines();
        }
   
        private void BeginAlternateLines(int interval)
        {
            ShouldBlink = true;
            _blinkTimer = new System.Timers.Timer(interval);
            _blinkTimer.Elapsed += TimerTick;
            _form.SwitchLineColor(0);
            _blinkTimer.Start();
        }

        private void StopAlternateLines()
        {
            _blinkTimer.Close();
            _checker.Close();
            _blinkTimer = null;
            _checker = null;

            ShouldBlink = false;

            if (blinkLine)
                _form.SwitchLineColor(_currentLine);
            else
                _form.SwitchColumnColor(_currentLine, _currentColumn);

            _currentLine = 0;
            _currentColumn = 0;
            blinkLine = true;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.WorkSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                        ReceiveCallback, state);
                    var indexBlink = state.Sb.ToString().IndexOf("BL:", StringComparison.CurrentCulture) + 3;
                    if (int.Parse(state.Sb.ToString().Substring(indexBlink, 1)) == 1)
                    {
                        ActivateKey();
                    }
                    state.Sb.Clear();
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.Sb.Length > 1)
                    {
                        //response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    //receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                //receiveDone.Set();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (ShouldBlink)
            {
                //If it's supposed to blink line switches its color, if not, switch columns color
                if (blinkLine)
                {
                    _form.SwitchLineColor(_currentLine);
                    _currentLine = (_currentLine + 1) % 5;
                    _form.SwitchLineColor(_currentLine);
                }
                else
                {
                    _form.SwitchColumnColor(_currentLine,_currentColumn);
                    _currentColumn = (_currentColumn + 1) % 12;
                    _form.SwitchColumnColor(_currentLine, _currentColumn);
                }
            }
        }

        private void ActivateKey()
        {
            _blinkTimer.Stop();
            //If was blinking lines change to blink columns, if was blinking columns press the current button
            if (blinkLine)
            {
                blinkLine = false;
                _form.SwitchLineColor(_currentLine); 
                _form.SwitchColumnColor(_currentLine,0); 
            }
            else
            {
                blinkLine = true;
                _form.SwitchColumnColor(_currentLine, _currentColumn);
                _form.ButtonClicked(_currentLine, _currentColumn);
                
                _currentLine = _currentColumn = 0;
                _form.SwitchLineColor(0);
            }
            _blinkTimer.Start();
        }
    }
}
