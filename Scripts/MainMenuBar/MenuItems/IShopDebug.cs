namespace BravoDebug.MainMenuBar.MenuItems
{
    public interface IShopDebug
    {
        public bool EnableDebug
        {
            get => _openWindow;
            set => _openWindow = value;
        }

        private static bool _openWindow;
        
        void InitializeDebug()
        {
            if (BravoDebugManager.Current == null) return;
        
            BravoDebugManager.Current.AddShopDebug(this);
            EnableDebug = false;
        }
        
        public void RenderDebug();
    }
}