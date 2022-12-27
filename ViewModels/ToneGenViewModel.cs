using ComPortApp.Commands;

namespace ComPortApp.ViewModels
{
    internal class ToneGenViewModel
    {
        public MakeToneCommand MakeToneCommand { get; } = new MakeToneCommand();
    }
}
