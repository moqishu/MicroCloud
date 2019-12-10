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
                return MyContainer.AddTransient<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddTransient<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            return MyContainer.AddTransient<TService, TImplementation>();
        }

        public static IServiceCollection AddSingleton<T>() where T : class
        {
            try
            {
                return MyContainer.AddSingleton<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddSingleton<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            return MyContainer.AddSingleton<TService, TImplementation>();
        }

        public static IServiceCollection AddScoped<T>() where T : class
        {
            try
            {
                return MyContainer.AddScoped<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IServiceCollection AddScoped<TService, TImplementation>() where TService : class
            where TImplementation : class, TService
        {
            return MyContainer.AddScoped<TService, TImplementation>();
        }

        /// <summary>
        /// 自动注册
        /// </summary>
        public static void AutoRegister()
        {
            List<Assembly> assemblys = LoadAssembly();

            List<Type> classTypes = GetClassTypes(assemblys);

            //实现ISingleton接口的类型集合注册为单例模式
            //List<Type> singletonTypeList = GetDerivedClass<ISingleton>(classTypes);
            ////注册单例
            //RegisterType<ContainerControlledLifetimeManager>(singletonTypeList);
        }

        public static List<Assembly> LoadAssembly()
        {
            List<string> assemblyFiles = GetAssemblyFiles();
            return AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assemblyFiles.Contains(assembly.ManifestModule.ScopeName)).ToList();
        }

        /// <summary>
        /// 获取指定目录及其子目录的所有DLL文件路径集合
        /// </summary>
        /// <param name="assemblyDirectory"></param>
        /// <returns></returns>
        private static List<string> GetAssemblyFiles()
        {
            string assemblyDirectory = AppDomain.CurrentDomain.BaseDirectory;
            //if (HttpContext.Current != null)
            //{
            //    assemblyDirectory = Path.Combine(assemblyDirectory, "Bin");
            //}
            assemblyDirectory = Path.Combine(assemblyDirectory, "Bin");
            string[] registerFiles = new string[]
            {
                "KT.Parking.Repositories.dll",
            };
            //获取DLL文件
            List<string> assemblyFiles = Directory.GetFiles(assemblyDirectory, "*.dll").Select(path => Path.GetFileName(path)).ToList();
            assemblyFiles = assemblyFiles.Where(f => registerFiles.Contains(f)).ToList();
            return assemblyFiles;
        }

        /// <summary>
        /// 从程序集加载所有类(不包含接口、抽象类)
        /// </summary>
        /// <param name="assemblys"></param>
        /// <returns></returns>
        private static List<Type> GetClassTypes(List<Assembly> assemblys)
        {
            List<Type> types = new List<Type>();
            assemblys.ForEach(assembly =>
            {
                try
                {
                    types.AddRange(assembly.GetTypes().Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract));
                }
                catch (ReflectionTypeLoadException ex)
                {
                    //处理类型加载异常，一般为缺少引用的程序集导致
                }

            });
            return types;
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
