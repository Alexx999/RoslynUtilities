using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace RoslynUtilities
{
    public static class SymbolExtensions
    {
        // (1) MAIN PATTERNS: TAP, EAP, APM
        public static bool IsTAPMethod(this IMethodSymbol symbol)
        {
            return (symbol.ReturnTask() && symbol.DeclaringSyntaxReferences.Count() == 0 && !symbol.ToString().StartsWith("System.Threading.Tasks") && !symbol.IsTaskCreationMethod())
                || (symbol.ReturnTask() && symbol.ToString().Contains("FromAsync"));

        }

        public static bool IsAPMBeginMethod(this IMethodSymbol symbol)
        {
            return !IsAsyncDelegate(symbol) && symbol.Parameters.Any(a=> a.Type.Name == "AsyncCallback") && !(symbol.ReturnsVoid) && symbol.ReturnType.Name == "IAsyncResult";
        }

        // (2) WAYS OF OFFLOADING THE WORK TO ANOTHER THREAD: TPL, THREADING, THREADPOOL, ACTION/FUNC.BEGININVOKE,  BACKGROUNDWORKER
        public static bool IsTaskCreationMethod(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("System.Threading.Tasks.Task.Start")
                || symbol.ToString().Contains("System.Threading.Tasks.Task.Run")
                || symbol.ToString().Contains("System.Threading.Tasks.TaskFactory.StartNew")
                || symbol.ToString().Contains("System.Threading.Tasks.TaskEx.RunEx")
                || symbol.ToString().Contains("System.Threading.Tasks.TaskEx.Run")
                || symbol.ToString().Contains("StartNewTask")
                || symbol.ToString().Contains("StartNewTaskWithoutExceptionHandling");

        }

        public static bool IsThreadPoolQueueUserWorkItem(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("ThreadPool.QueueUserWorkItem");
        }

        public static bool IsBackgroundWorkerMethod(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("BackgroundWorker.RunWorkerAsync");
        }

        public static bool IsThreadStart(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Thread.Start");
        }

        public static bool IsParallelFor(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Parallel.For") ||
                   symbol.ToString().Contains("ParallelWithMember.For");
        }
        public static bool IsParallelForEach(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Parallel.ForEach") ||
                   symbol.ToString().Contains("ParallelWithMember.ForEach");
        }

        public static bool IsParallelInvoke(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Parallel.Invoke") ||
                   symbol.ToString().Contains("ParallelLoader.Invoke");
        }

        public static bool IsAsyncDelegate(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Invoke") &&
                !(symbol.ReturnsVoid) && symbol.ReturnType.ToString().Contains("IAsyncResult");
        }

        // (3) WAYS OF UPDATING GUI: CONTROL.BEGININVOKE, DISPATCHER.BEGININVOKE, ISYNCHRONIZE.BEGININVOKE
        public static bool IsISynchronizeInvokeMethod(this IMethodSymbol symbol)
        {
            return symbol.ToString().StartsWith("System.ComponentModel.ISynchronizeInvoke");
        }

        public static bool IsControlBeginInvoke(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Control.BeginInvoke");
        }

        public static bool IsDispatcherBeginInvoke(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Dispatcher.BeginInvoke");
        }

        public static bool IsDispatcherInvoke(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("Dispatcher.Invoke");
        }

        // END
        public static bool IsAPMEndMethod(this IMethodSymbol symbol)
        {
            return symbol.ToString().Contains("IAsyncResult") && symbol.Name.StartsWith("End");
        }

        public static bool ReturnTask(this IMethodSymbol symbol)
        {
            return !symbol.ReturnsVoid && symbol.ReturnType.ToString().StartsWith("System.Threading.Tasks.Task");
        }

		public static MethodDeclarationSyntax FindMethodDeclarationNode(this IMethodSymbol methodCallSymbol)
		{
			if (methodCallSymbol == null)
				return null;

			var nodes = methodCallSymbol.DeclaringSyntaxReferences.Select(a => a.GetSyntax());

			if (nodes == null || nodes.Count() == 0)
				return null;

			var methodDeclarationNodes = nodes.OfType<MethodDeclarationSyntax>();

			if (methodDeclarationNodes.Count() != 0)
				return methodDeclarationNodes.First();

			return null;

			// above one is not always working. basically, above one is the shortcut for the below one!

			//var def = methodCallSymbol.FindSourceDefinition(currentSolution);

			//if (def != null && def.Locations != null && def.Locations.Count > 0)
			//{
			//    //methodCallSymbol.DeclaringSyntaxNodes.Firs
			//    var loc = def.Locations.First();

			//    Solution s;
			//    s.
			//    var node = loc.SourceTree.GetRoot().FindToken(loc.SourceSpan.Start).Parent;
			//    if (node is MethodDeclarationSyntax)
			//        return (MethodDeclarationSyntax)node;
			//}
		}
	}

}