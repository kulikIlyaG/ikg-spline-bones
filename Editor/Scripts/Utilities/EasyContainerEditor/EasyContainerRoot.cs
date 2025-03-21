using System.Collections.Generic;

namespace IKGTools.Editor.EasyContainer
{
    public static class EasyContainerRoot
    {
        private static DIContainer _rootContainer;
        
        public static void Initialize(IReadOnlyList<BaseInstaller> installers)
        {
            _rootContainer = new DIContainer();
            
            foreach (BaseInstaller installer in installers)
                installer.Install(_rootContainer);
            
            _rootContainer.Initialize();
        }
        

        public static DIContainer Container => _rootContainer;

        public static T Resolve<T>()
        {
            return _rootContainer.Resolve<T>();
        }

        public static void Register<T>(T instance)
        {
            _rootContainer.Register(instance);
        }
    }
}