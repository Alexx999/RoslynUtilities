using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RoslynUtilities
{
    public static class SyntaxExtensions
    {
		public static string ToLog(this SyntaxNode node)
		{
			return Environment.NewLine + Environment.NewLine + node.ToString()
				+ Environment.NewLine + Environment.NewLine;
        }

		// Extensions for MethodDeclaration nodes.
		public static bool IsAsync(this MethodDeclarationSyntax method)
        {
            return method.Modifiers.ToString().Contains("async") || method.ReturnType.ToString().Contains("Task");
        }

        public static bool IsTestMethod(this MethodDeclarationSyntax method)
        {
            return method.AttributeLists.Any(a => a.Attributes.ToString().Contains("TestMethod"));
        }

        public static bool HasEventArgsParameter(this MethodDeclarationSyntax method)
        {
            return method.ParameterList != null && method.ParameterList.Parameters.Any(param => param.Type.ToString().EndsWith("EventArgs"));
        }
        public static bool HasObjectStateParameter(this MethodDeclarationSyntax method)
        {
            return method.ParameterList != null && method.ParameterList.Parameters.Count == 1 && method.ParameterList.Parameters.First().Type.ToString() == "object";
        }

		public static bool IsEAPMethod(this InvocationExpressionSyntax invocation)
		{
			return invocation.Expression.ToString().ToLower().EndsWith("async") &&
				   invocation.Ancestors().OfType<MethodDeclarationSyntax>().Any(node =>
																		   node.DescendantNodes()
																		   .OfType<BinaryExpressionSyntax>()
																		   .Any(a => a.Left.ToString().ToLower().EndsWith("completed")));
		}

		public static bool ReturnsVoid(this MethodDeclarationSyntax method)
        {
            return method.ReturnType.ToString() == "void";
        }


        // EXTENSIONS FOR SYNTAXNODE
        /// <summary>
        /// Replace all old nodes in the given pairs with their corresponding new nodes.
        /// </summary>
        /// <typeparam name="T">Subtype of SyntaxNode that supports the
        /// replacement of descendent nodes.</typeparam>
        /// <param name="node">The SyntaxNode or subtype to operate on.</param>
        /// <param name="syntaxReplacementPairs">The SyntaxNodeReplacementPair
        /// instances that each contain both the old node that is to be
        /// replaced, and the new node that will replace the old node.</param>
        /// <returns>The SyntaxNode that contains all the replacmeents.</returns>
        public static T ReplaceAll<T>(this T node, params SyntaxReplacementPair[] syntaxReplacementPairs) where T : SyntaxNode
        {
            return node.ReplaceNodes(
                syntaxReplacementPairs.Select(pair => pair.OldNode),
                (oldNode, newNode) => syntaxReplacementPairs.First(pair => pair.OldNode == oldNode).NewNode
            );
        }

        /// <summary>
        /// Replace all old nodes in the given pairs with their corresponding new nodes.
        /// </summary>
        /// <typeparam name="T">Subtype of SyntaxNode that supports the
        /// replacement of descendent nodes.</typeparam>
        /// <param name="node">The SyntaxNode or subtype to operate on.</param>
        /// <param name="replacementPairs">The SyntaxNodeReplacementPair
        /// instances that each contain both the old node that is to be
        /// replaced, and the new node that will replace the old node.</param>
        /// <returns>The SyntaxNode that contains all the replacmeents.</returns>
        public static T ReplaceAll<T>(this T node, IEnumerable<SyntaxReplacementPair> replacementPairs) where T : SyntaxNode
        {
            return node.ReplaceNodes(
                replacementPairs.Select(pair => pair.OldNode),
                (oldNode, newNode) => replacementPairs.First(pair => pair.OldNode == oldNode).NewNode
            );
        }
    }

    /// <summary>
    /// Pair of old and new SyntaxNodes for ReplaceAll.
    /// </summary>
    public sealed class SyntaxReplacementPair
    {
        /// <summary>The node that must be replaced.</summary>
        public readonly SyntaxNode OldNode;

        /// <summary>The node that will replace the old node.</summary>
        public readonly SyntaxNode NewNode;

        public SyntaxReplacementPair(SyntaxNode oldNode, SyntaxNode newNode)
        {
            if (oldNode == null) throw new ArgumentNullException("oldNode");
            if (newNode == null) throw new ArgumentNullException("newNode");

            OldNode = oldNode;
            NewNode = newNode;
        }
    }

}
