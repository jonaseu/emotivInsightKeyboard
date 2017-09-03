﻿using System;
using System.Collections.Specialized;
using System.Diagnostics;
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
        private readonly frmKeyboard _form;
        
        private System.Timers.Timer _blinkTimer;
        private System.Timers.Timer _checker;
        private Stopwatch _stopWatch;
        private int _currentLine;
        private int _currentColumn;
        private int _numberColumns;
        private bool _shouldBlink;
        private bool _blinkLine;
        

        //Emotiv Configs
        private Socket _sckEmoEngine = null;
        private string _ipToConnect = "";
        private string _clickMode = "";
        private int _portToConnect = 0;
        private int _sensitivity = 0;
        private int _timeInterval = 0;

        public rulKeyboard(frmKeyboard frm)
        {
            _form = frm;
            _currentLine = 0;
            _currentColumn = 0;
            _numberColumns = 12;
            _shouldBlink = false;
            _blinkLine = true;
            _stopWatch = new Stopwatch();
        }

        public bool ConnectEmotiv(string host, int port,string clickMode, int interval, int sensitivity)
        {

            _clickMode = clickMode;
            _timeInterval = interval;
            _sensitivity = sensitivity;
            _ipToConnect = host;
            _portToConnect = port;
            _sckEmoEngine = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            StateObject state = new StateObject { WorkSocket = _sckEmoEngine };

            try
            {
                _sckEmoEngine.Connect(_ipToConnect, _portToConnect);
                _sckEmoEngine.SendTimeout = 2000;
                byte[] toBytes = Encoding.ASCII.GetBytes("Mode:"+_clickMode+"  Interval:"+_timeInterval+"  Sensitivity:"+_sensitivity);
                _sckEmoEngine.Send(toBytes);
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
            _shouldBlink = true;
            _blinkTimer = new System.Timers.Timer(interval);
            _blinkTimer.Elapsed += TimerTick;
            _timeInterval = interval;
            _form.SwitchLineColor(0);
            _blinkTimer.Start();
        }

        private void StopAlternateLines()
        {
            _blinkTimer.Close();
            _checker.Close();
            _blinkTimer = null;
            _checker = null;

            _shouldBlink = false;

            if (_blinkLine)
                _form.SwitchLineColor(_currentLine);
            else
                _form.SwitchColumnColor(_currentLine, _currentColumn);

            _currentLine = 0;
            _currentColumn = 0;
            _blinkLine = true;
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
            try
            {
                _blinkTimer.Stop();
                _blinkTimer.Interval = _timeInterval;
                if (_shouldBlink)
                {
                    //If it's supposed to blink line switches its color, if not, switch columns color
                    if (_blinkLine)
                    {
                        _form.SwitchLineColor(_currentLine);
                        _currentLine = (_currentLine + 1) % 5;
                        _form.SwitchLineColor(_currentLine);
                    }
                    else
                    {
                        _numberColumns = _form.SwitchColumnColor(_currentLine, _currentColumn);
                        _currentColumn = (_currentColumn + 1) % _numberColumns;
                        _form.SwitchColumnColor(_currentLine, _currentColumn);
                    }
                }
                _stopWatch.Start();
                _blinkTimer.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        private void ActivateKey()
        {
            _stopWatch.Stop();
            _blinkTimer.Stop();
            _blinkTimer.Interval = _timeInterval * 1.5;
            //If was blinking lines change to blink columns, if was blinking columns press the current button
            if (_blinkLine)
            {
                
                _blinkLine = false;
                _form.SwitchLineColor(_currentLine);
                if (_stopWatch.Elapsed.TotalMilliseconds < 100)
                {
                    _currentLine = mod(_currentLine - 1, 5);
                }


                _form.SwitchColumnColor(_currentLine,0);
            }
            else
            {
                
                _form.SwitchColumnColor(_currentLine, _currentColumn);
                
                if (_stopWatch.Elapsed.Milliseconds < 100)
                {
                    _currentColumn = mod(_currentColumn - 1, _numberColumns);
                }

                _blinkLine = true;

                _form.ButtonClicked(_currentLine, _currentColumn);
                
                _currentLine = _currentColumn = 0;
                _form.SwitchLineColor(0);
            }
            _blinkTimer.Start();
        }

        private int mod(int k, int n) { return ((k %= n) < 0) ? k + n : k; }
    }  
}
