using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using IKGTools.Editor.EasyContainerEditor.Services;

using UnityEditor;

namespace IKGTools.Editor.EasyContainerEditor
{
    public sealed class DIContainer
    {
        private readonly DIContainer _parentContainer;
        
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();

        private readonly HashSet<IInitializable> _initializableInstances = new HashSet<IInitializable>();
        private readonly HashSet<ITickable> _tickableInstances = new HashSet<ITickable>();
        private readonly HashSet<ITickableSceneGUI> _tickableSceneGUIInstances = new HashSet<ITickableSceneGUI>();

        internal DIContainer() { }

        internal DIContainer(DIContainer parent)
        {
            _parentContainer = parent;
            var eventsHelper = _parentContainer.Resolve<ContainerEventsService>();
            
            eventsHelper.OnTick += OnTick;
            eventsHelper.OnTickSceneGUI += OnTickSceneGUI;
        }
        
        public void CreateAndRegister<T>()
        {
            var instance = CreateInstance<T>();
            Register(instance);
        }

        public void Register<T>(T instance)
        {
            _instances[typeof(T)] = instance;
            
            if (instance is IInitializable instanceInitializable)
                _initializableInstances.Add(instanceInitializable);

            if (instance is ITickable instanceTickable)
                _tickableInstances.Add(instanceTickable);

            if (instance is ITickableSceneGUI instanceTickableSceneGUI)
                _tickableSceneGUIInstances.Add(instanceTickableSceneGUI);
        }
        
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            if (_instances.TryGetValue(type, out var instance))
                return instance;

            if (_parentContainer != null)
                return _parentContainer.Resolve(type);

            throw new Exception($"Not found bindings for {type.Name}");
        }

        public bool IsRegistered<T>()
        {
            return IsRegistered(typeof(T));
        }

        public bool IsRegistered(Type type)
        {
            if (_instances.ContainsKey(type))
            {
                return true;
            }

            if (_parentContainer != null)
            {
                return _parentContainer.IsRegistered(type);
            }

            return false;
        }

        public DIContainer CreateSubContext(IReadOnlyList<BaseInstaller> installers)
        {
            var container = new DIContainer(this);
            if (installers is {Count: > 0})
            {
                foreach (var installer in installers)
                    installer.Install(container);

                container.Initialize();
            }
            return container;
        }
        

        private void OnTickSceneGUI(SceneView obj)
        {
            foreach (ITickableSceneGUI instance in _tickableSceneGUIInstances)
                instance.TickSceneGUI(obj);
        }

        private void OnTick()
        {
            foreach (ITickable instance in _tickableInstances)
                instance.Tick();
        }

        internal void Initialize()
        {
            foreach (IInitializable instance in _initializableInstances)
                instance.Initialize();
        }
        
        private T CreateInstance<T>()
        {
            return (T)CreateInstance(typeof(T));
        }

        private object CreateInstance(Type type)
        {
            object instance = CreateInstanceViaConstructor(type);
            
            if (instance == null)
            {
                if (type.GetConstructor(Type.EmptyTypes) == null)
                {
                    throw new Exception($"Type {type.Name} don't have default constructor.");
                }
                
                instance = Activator.CreateInstance(type);
            }
            
            InjectInto(instance);
            
            return instance;
        }

        private object CreateInstanceViaConstructor(Type type)
        {
            var constructors = type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => c.GetCustomAttribute<EasyInjectAttribute>() != null)
                .ToList();
            
            if (constructors.Count == 0)
                return null;
            
            if (constructors.Count > 1)
                throw new Exception($"Type {type.Name} has multiple constructors with the [EasyInject] attribute. Only one constructor can be marked for injection.");
            
            var constructor = constructors[0];
            var injectAttribute = constructor.GetCustomAttribute<EasyInjectAttribute>();
            
            try
            {
                var parameters = constructor.GetParameters();
                var arguments = new object[parameters.Length];
                
                for (int i = 0; i < parameters.Length; i++)
                {
                    var paramType = parameters[i].ParameterType;
                    if (IsRegistered(paramType))
                    {
                        arguments[i] = Resolve(paramType);
                    }
                    else if (!injectAttribute.Optional)
                    {
                        throw new Exception($"Parameter {parameters[i].Name} of constructor of type {type.Name} requires type {paramType.Name}, but it is not registered in the container.");
                    }
                }
                
                return constructor.Invoke(arguments);
            }
            catch (Exception e)
            {
                if (!injectAttribute.Optional)
                    throw new Exception($"Error creating instance of type {type.Name} via constructor with injection: {e.Message}");
                
                return null;
            }
        }

        private void InjectInto(object instance)
        {
            if (instance == null)
                return;

            var type = instance.GetType();

            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var injectAttribute = field.GetCustomAttribute<EasyInjectAttribute>();
                if (injectAttribute != null)
                {
                    try
                    {
                        var fieldType = field.FieldType;
                        if (IsRegistered(fieldType))
                        {
                            field.SetValue(instance, Resolve(fieldType));
                        }
                        else if (!injectAttribute.Optional)
                        {
                            throw new Exception(
                                $"Field {field.Name} is marked for injection, but type {fieldType.Name} is not registered in the container");
                        }
                    }
                    catch (Exception e)
                    {
                        if (!injectAttribute.Optional)
                            throw new Exception($"Error injecting into field {field.Name}: {e.Message}");
                    }
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public |
                                                        BindingFlags.NonPublic))
            {
                var injectAttribute = property.GetCustomAttribute<EasyInjectAttribute>();
                if (injectAttribute != null && property.CanWrite)
                {
                    try
                    {
                        var propertyType = property.PropertyType;
                        if (IsRegistered(propertyType))
                        {
                            property.SetValue(instance, Resolve(propertyType));
                        }
                        else if (!injectAttribute.Optional)
                        {
                            throw new Exception(
                                $"Property {property.Name} is marked for injection, but type {propertyType.Name} is not registered in the container");
                        }
                    }
                    catch (Exception e)
                    {
                        if (!injectAttribute.Optional)
                            throw new Exception($"Error injecting into property {property.Name}: {e.Message}");
                    }
                }
            }

            foreach (var method in
                     type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var injectAttribute = method.GetCustomAttribute<EasyInjectAttribute>();
                if (injectAttribute != null)
                {
                    try
                    {
                        var parameters = method.GetParameters();
                        var arguments = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var paramType = parameters[i].ParameterType;
                            if (IsRegistered(paramType))
                            {
                                arguments[i] = Resolve(paramType);
                            }
                            else if (!injectAttribute.Optional)
                            {
                                throw new Exception(
                                    $"Parameter {parameters[i].Name} of method {method.Name} requires type {paramType.Name}, but it is not registered in the container");
                            }
                        }

                        method.Invoke(instance, arguments);
                    }
                    catch (Exception e)
                    {
                        if (!injectAttribute.Optional)
                            throw new Exception($"Error injecting via method {method.Name}: {e.Message}");
                    }
                }
            }
        }
    }
}