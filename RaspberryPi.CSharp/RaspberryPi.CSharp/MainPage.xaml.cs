using System;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.System.Threading;
using System.Collections.Generic;
using Windows.Storage;


namespace RaspberryPi.CSharp
{
    public sealed partial class MainPage : Page
    {
        private int totalTransactions = 0;
        private double averageTimePerTransaction = 0;
        private double totalTime = 0;
        private double elapsedTime = 0;
        private ThreadPoolTimer timer;
        private ThreadPoolTimer timerBlinkRed;
        private ThreadPoolTimer timerBlinkGreen;
        private int totalToBlinkGreen = 0;
        private int totalToBlinkRed = 0;
        CustomGPIOController customGPIOController;
        List<string> list;
        private Object thisLock = new object();
        private bool NextActionIsShowTransactionTiming = false;
        private readonly int timeBetweenPing = 10000;
        private readonly int timeBetweenLEDFlash = 100;

        private void Timer_Tick_Blink_Green(ThreadPoolTimer timer)
        {
            lock(thisLock)
            {
                totalToBlinkGreen--;
                if (totalToBlinkGreen == 0)
                {
                    timerBlinkGreen.Cancel();
                    if (NextActionIsShowTransactionTiming)
                    {
                        totalToBlinkGreen = 10;
                        totalToBlinkRed = 10;
                        NextActionIsShowTransactionTiming = false;
                        customGPIOController.TurnLEDsOff();
                        ShowTransactionTiming(elapsedTime);
                    }
                    else
                    {
                        WriteDataToFile(list);
                        customGPIOController.TurnLEDsOff();
                        timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(timeBetweenPing));
                    }
                }
                else
                {
                    if (customGPIOController.greenPinValue == Windows.Devices.Gpio.GpioPinValue.High)
                    {
                        customGPIOController.ToggleGreen(true);
                    }
                    else
                    {
                        customGPIOController.ToggleGreen(false);
                    }
                }
            }
        }

        private void Timer_Tick_Blink_Red(ThreadPoolTimer timer)
        {
            lock (thisLock)
            {
                totalToBlinkRed--;
                if (totalToBlinkRed == 0)
                {
                    timerBlinkRed.Cancel();
                    if (NextActionIsShowTransactionTiming)
                    {
                        totalToBlinkGreen = 10;
                        totalToBlinkRed = 10;
                        NextActionIsShowTransactionTiming = false;
                        customGPIOController.TurnLEDsOff();
                        ShowTransactionTiming(elapsedTime);
                    }
                    else
                    {
                        WriteDataToFile(list);
                        customGPIOController.TurnLEDsOff();
                        timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(timeBetweenPing));
                    }
                    
                }
                else
                {
                    if (customGPIOController.redPinValue == Windows.Devices.Gpio.GpioPinValue.High)
                    {
                        customGPIOController.ToggleRed(true);
                    }
                    else
                    {
                        customGPIOController.ToggleRed(false);
                    }
                }
            }
        }

        private void ShowTransactionSuccess(bool approved)
        {
            if (approved)
            { 
                timerBlinkGreen = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick_Blink_Green, TimeSpan.FromMilliseconds(timeBetweenLEDFlash));
            }
            else
            {
                timerBlinkRed = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick_Blink_Red, TimeSpan.FromMilliseconds(timeBetweenLEDFlash));
            }
        }

        private void ShowTransactionTiming(double elapsedTime)
        {
            if (elapsedTime <= .5)
            {
                timerBlinkGreen = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick_Blink_Green, TimeSpan.FromMilliseconds(timeBetweenLEDFlash));
            }
            else
            {
                timerBlinkRed = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick_Blink_Red, TimeSpan.FromMilliseconds(timeBetweenLEDFlash));
            }
        }

        private void Timer_Tick(ThreadPoolTimer timer)
        {
            timer.Cancel();

            list.Clear();

            var sw = new Stopwatch();
            sw.Start();

            var mercury = new MercuryAccess();
            var success = mercury.CallMPS().Result;
            sw.Stop();

            elapsedTime = (double)sw.ElapsedMilliseconds/(double)1000.0;            

            list.Add(string.Format("{0}    ElapsedTime: {1}", DateTime.Now, elapsedTime));

            totalTime += elapsedTime;

            list.Add(string.Format("{0}    TotalTime: {1}", DateTime.Now, totalTime));

            totalTransactions++;

            list.Add(string.Format("{0}    Total Transactions: {1}", DateTime.Now, totalTransactions));

            averageTimePerTransaction = (double)totalTime / (double)totalTransactions;

            list.Add(string.Format("{0}    Average Transaction Time: {1}", DateTime.Now, averageTimePerTransaction));

            totalToBlinkGreen = 10;
            totalToBlinkRed = 10;
            NextActionIsShowTransactionTiming = true;

            ShowTransactionSuccess(success);            
        }

        private async void WriteDataToFile(List<string> data)
        {
            StorageFolder folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile sampleFile = await folder.CreateFileAsync("logging.txt", CreationCollisionOption.OpenIfExists);

            await Windows.Storage.FileIO.WriteLinesAsync(sampleFile, data);
        }

        public MainPage()
        {
            InitializeComponent();
            customGPIOController = new CustomGPIOController();                        
            list = new List<string>();

            timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(timeBetweenPing));      
        }
    }
}
