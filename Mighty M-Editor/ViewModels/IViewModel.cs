using Mighty_M_Editor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mighty_M_Editor.ViewModels
{
	public interface IViewModel<T> : IViewModel where T : IView
	{
		new T View
		{
			get;
			set;
		}
	}

	public interface IViewModel
	{
		object View
		{
			get;
			set;
		}
	}
}
