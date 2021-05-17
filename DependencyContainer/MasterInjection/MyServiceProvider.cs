namespace MasterInjection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MyServiceProvider : IMyServiceProvider
    {
        private readonly Dictionary<Type, Type> _types;

        public MyServiceProvider()
        {
            _types = new Dictionary<Type, Type>();
        }

        public void Add<TSource, TDestination>()
            where TDestination : TSource
        {
            _types[typeof(TSource)] = typeof(TDestination);
        }

        public object CreateInstance(Type type)
        {
            if (!_types.ContainsKey(type) && !_types.ContainsValue(type))
            {
                return null;
            }

            var ctors = type.GetConstructors();
            var ctor = ctors.FirstOrDefault();
            var ctorParams = GetParameter(type);
            var ctorInvoke = ctor.Invoke(ctorParams);
            
            if (ctorInvoke != null)
            {
                return ctorInvoke;
            }

            return Activator.CreateInstance(type);
        }

        public T CreateInstance<T>()
        {
            T obj = (T)CreateInstance(typeof(T));

            return obj;
        }

        private object[] GetParameter(Type typeData)
        {
            var constructorsOfType = typeData.GetConstructors();
            var firstConstructor = constructorsOfType.FirstOrDefault();
            var parametersInConstructor = firstConstructor.GetParameters();
            var result = new List<object>();
            foreach (var param in parametersInConstructor)
            {
                var paramType = param.ParameterType;
                if (paramType.IsInterface)
                {
                    var classList = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(w => paramType.IsAssignableFrom(w) & !w.IsInterface).ToList();

                    var type = classList.FirstOrDefault();

                    var parameteDatar = GetParameter(type);

                    var instanceOfImplement = (parameteDatar == null || parameteDatar.Length == 0)
                        ?
                        Activator.CreateInstance(type)
                        :
                        Activator.CreateInstance(type, parameteDatar);

                    result.Add(instanceOfImplement);
                }
            }
            return result.ToArray();
        }
    }
}
