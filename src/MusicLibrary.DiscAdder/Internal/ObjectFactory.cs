﻿using System;
using Microsoft.Extensions.DependencyInjection;
using MusicLibrary.DiscAdder.Interfaces;

namespace MusicLibrary.DiscAdder.Internal
{
	internal class ObjectFactory<TType> : IObjectFactory<TType>
		where TType : class
	{
		private readonly IServiceProvider serviceProvider;

		public ObjectFactory(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
		}

		public TType CreateInstance()
		{
			return serviceProvider.GetRequiredService<TType>();
		}
	}
}
