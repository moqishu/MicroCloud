using System;

namespace LIN.MSA.Infrastructure
{
    public class ScheduleEntity
    {
        /// <summary>
        /// 开始执行时间
        /// </summary>
        public DateTime StarRunTime { get; set; }
        /// <summary>
        /// 结束执行时间
        /// </summary>
        public DateTime? EndRunTime { get; set; }
        /// <summary>
        /// 作业名称
        /// </summary>
        public string JobName { get; set; }
        /// <summary>
        /// 触发器名称
        /// </summary>
        public string JobGroup { get; set; }
        /// <summary>
        /// 间隔表达式
        /// </summary>
        public string CronStr { get; set; }

        /// <summary>
        /// 方式:0 表达式；1：时分秒
        /// </summary>
        public ScheduleType Type { get; set; }

        public int Span { get; set; }
    }
}
