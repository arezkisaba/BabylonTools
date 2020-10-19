using System;
using System.Diagnostics;
using System.IO;
using J2i.Net.XInputWrapper;
using System.Threading.Tasks;
using Lib.Win32;
using Lib.Core;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BabylonTools.XMap
{
    public class XMap
    {
        private IDisplayService _displayService;
        private XboxController _xboxController;
        private List<string> _gameProcesses;

        public XMap()
        {
            _displayService = new DisplayService();
            _xboxController = XboxController.RetrieveController(0);
            _gameProcesses = ConfigurationManager.AppSettings["gameProcesses"].Split(';').ToList();
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var foregroundProcess = ProcessesHelper.GetForegroundProcess();
                        if (foregroundProcess == null)
                        {
                            continue;
                        }

                        if (_xboxController.IsLeftShoulderPressed &&
                            _xboxController.LeftTrigger > 0 &&
                            _xboxController.IsRightShoulderPressed &&
                            _xboxController.RightTrigger > 0 &&
                            _xboxController.IsAPressed)
                        {
                            foreach (var gameProcess in _gameProcesses)
                            {
                                if (foregroundProcess.ProcessName == gameProcess)
                                {
                                    Console.WriteLine($"========================================");
                                    Console.WriteLine($"Foreground process : {foregroundProcess.ProcessName}");
                                    ProcessesHelper.Stop(gameProcess);
                                    Console.WriteLine($"{gameProcess} killed");
                                    await Task.Delay(1000);

                                    var oldDisplay = _displayService.GetCurrent();
                                    var newDisplay = _displayService.GetHigherDisplay();

                                    if (oldDisplay.Width < newDisplay.Width)
                                    {
                                        Console.WriteLine($"old display : {oldDisplay.Width} x {oldDisplay.Height}");
                                        _displayService.SetCurrent(newDisplay);
                                        Console.WriteLine($"new display : {newDisplay.Width} x {newDisplay.Height}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    await Task.Delay(500);
                }
            });

            Task.Run(() =>
            {
                XboxController.StartPolling(); 
            });
        }

        public void Stop()
        {
            XboxController.StopPolling();
        }
    }
}