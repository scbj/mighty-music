using Mighty_M_Editor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Mighty_M_Editor.Views
{
	public interface IView<T> : IView
	where T : IViewModel
	{
		new T ViewModel
		{
			get;
			set;
		}
	}

	public interface IView
	{
		object ViewModel
		{
			get;
			set;
		}

		Dispatcher Dispatcher
		{
			get;
		}
	}
}
