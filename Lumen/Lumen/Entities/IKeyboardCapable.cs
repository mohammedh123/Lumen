namespace Lumen.Entities
{
    internal interface IKeyboardCapable
    {
        void ProcessKeyDownAction(float dt);
        void ProcessKeyUpAction(float dt);
        void ProcessKeyPressAction(float dt);
    }
}