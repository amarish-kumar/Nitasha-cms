using NITASA.Core.Plugins;
using NITASA.Core.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NITASA.Core.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return Mapper.Map<TSource, TDestination>(source);
        }


        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return entity.MapTo<PluginDescriptor, PluginModel>();
        }
    }
}