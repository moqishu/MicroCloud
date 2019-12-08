using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;

namespace LIN.MSA.Infrastructure
{
    public class ConfigHelper
    {
        //注意：section为根节点
        private string _jsonName;
        private string _path;
        private IConfiguration Configuration { get; set; }
        public ConfigHelper(string jsonName)
        {
            _jsonName = jsonName;
            if (!jsonName.EndsWith(".json"))
                _path = $"{jsonName}.json";
            else
                _path = jsonName;
            //ReloadOnChange = true 当*.json文件被修改时重新加载            
            Configuration = new ConfigurationBuilder()
            .Add(new JsonConfigurationSource { Path = _path, ReloadOnChange = true, Optional = true })
            .Build();
        }

    }
}
