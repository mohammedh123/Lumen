namespace Lumen.Entities
{
    interface IKeyboardCapable
    {
        void ProcessKeyDownAction(float dt);
        void ProcessKeyUpAction(float dt);
        void ProcessKeyPressAction(float dt);
    }
}
