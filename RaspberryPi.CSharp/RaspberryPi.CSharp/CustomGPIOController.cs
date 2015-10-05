using Windows.Devices.Gpio;

namespace RaspberryPi.CSharp
{
    public class CustomGPIOController
    {
        private const int RED_LED_PIN = 5;
        private const int GREEN_LED_PIN = 12;
        private GpioPin greenPin;
        private GpioPin redPin;
        public GpioPinValue greenPinValue;
        public GpioPinValue redPinValue;

        public CustomGPIOController()
        {
            InitGPIO();
        }

        public void ToggleRed(bool on)
        {
            if (on)
            {
                redPinValue = GpioPinValue.Low;                
            }
            else
            {
                redPinValue = GpioPinValue.High;
            }

            redPin.Write(redPinValue);
        }

        public void ToggleGreen(bool on)
        {
            if (on)
            {
                greenPinValue = GpioPinValue.Low;                
            }
            else
            {
                greenPinValue = GpioPinValue.High;
            }
            greenPin.Write(greenPinValue);
        }

        public void TurnLEDsOff()
        {
            greenPinValue = GpioPinValue.High;
            greenPin.Write(greenPinValue);
            redPinValue = GpioPinValue.High;
            redPin.Write(redPinValue);
        }
      
        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                greenPin = null;
                redPin = null;
                return;
            }

            redPin = gpio.OpenPin(RED_LED_PIN);
            redPinValue = GpioPinValue.High;
            redPin.Write(redPinValue);
            redPin.SetDriveMode(GpioPinDriveMode.Output);

            greenPin = gpio.OpenPin(GREEN_LED_PIN);
            greenPinValue = GpioPinValue.High;
            greenPin.Write(greenPinValue);
            greenPin.SetDriveMode(GpioPinDriveMode.Output);
        }
    }
}
