using ArduinoControlApp.Commands;

namespace ArduinoControlApp.ViewModels
{
    internal class ToneGenViewModel
    {
        public MakeToneCommand MakeToneCommand { get; } = new MakeToneCommand();
    }
}
