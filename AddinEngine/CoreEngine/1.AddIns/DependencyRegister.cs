using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AddinEngine.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AddinEngine
{
    public class DependencyRegister : IDependencyRegister  
    {  
        private readonly IServiceCollection serviceCollection;  
  
        public DependencyRegister(IServiceCollection serviceCollection)  
        {  
            this.serviceCollection = serviceCollection;  
        }  
  
        // void IDependencyRegister.AddScopedForMultiImplementation<TService, TImplementation>()  
        // {  
        //     serviceCollection.AddScoped<TImplementation>()  
        //         .AddScoped<TService, TImplementation>(s => s.GetService<TImplementation>());  
        // }  
  
        // void IDependencyRegister.AddTransientForMultiImplementation<TService, TImplementation>()  
        // {  
        //     serviceCollection.AddTransient<TImplementation>()  
        //         .AddTransient<TService, TImplementation>(s => s.GetService<TImplementation>());  
        // }

        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            return serviceCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) serviceCollection).GetEnumerator();
        }

        public void Add(ServiceDescriptor item)
        {
            serviceCollection.Add(item);
        }

        public void Clear()
        {
            serviceCollection.Clear();
        }

        public bool Contains(ServiceDescriptor item)
        {
            return serviceCollection.Contains(item);
        }

        public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
        {
            serviceCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(ServiceDescriptor item)
        {
            return serviceCollection.Remove(item);
        }

        public int Count => serviceCollection.Count;

        public bool IsReadOnly => serviceCollection.IsReadOnly;

        public int IndexOf(ServiceDescriptor item)
        {
            return serviceCollection.IndexOf(item);
        }

        public void Insert(int index, ServiceDescriptor item)
        {
            serviceCollection.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            serviceCollection.RemoveAt(index);
        }

        public ServiceDescriptor this[int index]
        {
            get => serviceCollection[index];
            set => serviceCollection[index] = value;
        }
    }  
}
