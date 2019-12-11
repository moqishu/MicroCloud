using AspectCore.Configuration;
using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace LIN.MSA.Infrastructure
{
    /// <summary>
    /// IOC容器
    /// </summary>
    public class IocContainer
    {
        private static IServiceCollection _myContainer;
        private static IServiceProvider _serviceProvider;

        //AddSingleton→AddTransient→AddScoped
        //AddSingleton的生命周期：
        //项目启动-项目关闭 相当于静态类  只会有一个
        //AddScoped的生命周期：
        //请求开始-请求结束 在这次请求中获取的对象都是同一个
        //AddTransient的生命周期：
        //请求获取-（GC回收-主动释放） 每一次获取的对象都不是同一个
        //config.Interceptors.AddTyped<CustomInterceptor>(Predicates.ForMethod("*Query")); //拦截所有Query后缀的方法
        //config.Interceptors.AddTyped<CustomInterceptor>(Predicates.ForService("*Repository")); //拦截所有Repository后缀的类或接口
        //config.Interceptors.AddTyped<CustomInterceptor>(Predicates.ForMethod("AspectCoreDemo.*")); //拦截所有AspectCoreDemo及其子命名空间下面的接口或类

        public static IServiceCollection MyContainer
        {
            get
            {
                if (_myContainer == null)
                {
                    _myContainer = new ServiceCollection();
                }

                return _myContainer;
            }
        }

        public static IServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    _serviceProvider = MyContainer.BuildDynamicProxyProvider();
                }

                return _serviceProvider;
            }
        }

        public static T GetService<T>() where T : class
        {
            try
            {
                return ServiceProvider.GetService<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddTransient<T>() where T : class
        {
            try
            {
                // 手动注册需要重新 BuildDynamicProxyProvider()否则获取不到对象
                var result = MyContainer.AddTransient<T>();
                _serviceProvider = MyContainer.BuildDynamicProxyProvider();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddTransient<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            var result = MyContainer.AddTransient<TService, TImplementation>();
            _serviceProvider = MyContainer.BuildDynamicProxyProvider();
            return result;
        }

        public static IServiceCollection AddSingleton<T>() where T : class
        {
            try
            {
                var result = MyContainer.AddSingleton<T>();
                _serviceProvider = MyContainer.BuildDynamicProxyProvider();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            var result = MyContainer.AddSingleton<TService, TImplementation>();
            _serviceProvider = MyContainer.BuildDynamicProxyProvider();
            return result;
        }

        public static IServiceCollection AddScoped<T>() where T : class
        {
            try
            {
                var result = MyContainer.AddScoped<T>();
                _serviceProvider = MyContainer.BuildDynamicProxyProvider();
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddScoped<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            var result = MyContainer.AddScoped<TService, TImplementation>();
            _serviceProvider = MyContainer.BuildDynamicProxyProvider();
            return result;
        }

        /// <summary>
        /// 自动注册
        /// </summary>
        public static void AutoRegister()
        {
            var classNames = GetAssembly();
            foreach (var item in classNames)
            {
                // 存在接口的注册方式
                if (item.Value.Length > 0)
                {
                    foreach (var typeArry in item.Value)
                    {
                        // 1.实例注册
                        MyContainer.AddScoped(item.Key, typeArry);
                    }
                }
                else
                {
                    MyContainer.AddScoped(item.Key);
                }

                if (item.Key.BaseType != null && item.Key.BaseType.FullName == typeof(AbstractInterceptor).ToString())
                {
                    // 2.全局拦截
                    MyContainer.ConfigureDynamicProxy(config =>
                    {
                        config.Interceptors.AddTyped(item.Key);
                    });
                }
            }

            _serviceProvider = MyContainer.BuildDynamicProxyProvider();
        }

        /// <summary>
        /// 获取目录程序集
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type, Type[]> GetAssembly()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;

            //获取DLL文件
            List<string> files = Directory.GetFiles(directory, "*.dll").Select(path => Path.GetFileName(path)).ToList();
            files = files.Where(f => f.Contains("LIN.MSA.")).ToList();

            List<Assembly> dllFiles = new List<Assembly>();
            foreach (var file in files)
            {
                dllFiles.Add(Assembly.Load(file.Replace(".dll", "")));
            }

            List<Type> types = new List<Type>();

            foreach (var item in dllFiles)
            {
                types.AddRange(item.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract));
            }

            Dictionary<Type, Type[]> result = new Dictionary<Type, Type[]>();
            foreach (var key in types)
            {
                var interfaceType = key.GetInterfaces();
                result.Add(key, interfaceType);
            }

            return result;
        }

        /// <summary>
        /// 获取目录程序集
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Type, Type[]> GetAssembly2()
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory;
            //directory = Path.Combine(directory, "Bin");
            string[] registerFiles = new string[]
            {
                "LIN.MSA.DataAccess.dll",
            };
            //获取DLL文件
            List<string> files = Directory.GetFiles(directory, "*.dll").Select(path => Path.GetFileName(path)).ToList();
            files = files.Where(f => registerFiles.Contains(f)).ToList();

            // 程序运行时程序集
            var dllFiles = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => files.Contains(assembly.ManifestModule.ScopeName)).ToList();

            List<Type> types = new List<Type>();

            foreach (var item in dllFiles)
            {
                types.AddRange(item.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract));
            }

            Dictionary<Type, Type[]> result = new Dictionary<Type, Type[]>();
            foreach (var key in types)
            {
                var interfaceType = key.GetInterfaces();
                result.Add(key, interfaceType);
            }

            return result;
        }

        /// <summary>
        /// 从指定的类型集合中过滤出从指定类或接口派生的类
        /// </summary>
        /// <typeparam name="T">基类或接口</typeparam>
        /// <param name="classTypes">类型集合</param>
        /// <returns></returns>
        private static List<Type> GetDerivedClass<T>(List<Type> classTypes) where T : class
        {
            return classTypes.AsParallel().Where(t => t.GetInterface(typeof(T).ToString()) != null).ToList();
        }

    }
}
