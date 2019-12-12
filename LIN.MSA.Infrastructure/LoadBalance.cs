using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LIN.MSA.Infrastructure
{
    /// <summary>
    /// 负载均衡
    /// </summary>
    public class LoadBalance
    {
        public static int Balance = 0;

        /// <summary>
        /// 轮询法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ServiceEntity RoundRobin(List<ServiceEntity> list)
        {
            Balance = Balance % list.Count;
            var result = list[Balance];
            Balance = Balance + 1;

            return result;
        }

        /// <summary>
        /// 加权轮询法
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public ServiceEntity WeightRoundRobin(List<ServiceEntity> list)
        {
            foreach (var service in list)
            {
                // 权重100 生成100条数据
                list.AddRange(Enumerable.Repeat(service, service.Weight));
            }
            Balance = Balance % list.Count;
            var result = list[Balance];
            Balance = Balance + 1;

            return result;
        }




    }

    public class ServiceEntity
    {

        public string Name { get; set; }

        public string IP { get; set; }

        public string Port { get; set; }

        public bool Health { get; set; }

        public int Weight { get; set; }

        public string CheckMethod { get; set; }
    }
}
