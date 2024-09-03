using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Data;

namespace TaleworldsCodeAnalysis
{
    public class AnalyzerDisablingComments
    {
        private static AnalyzerDisablingComments _instance;
        public static AnalyzerDisablingComments Instance { get {
                if (_instance == null) return new AnalyzerDisablingComments();
                else return _instance;
            }
        }

        private AnalyzerDisablingComments() { }

        public bool IsInDisablingComments(SyntaxNode node, String diagnosticId) {
            var root = node.SyntaxTree.GetRoot();
            var singleLineComments = root.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia));

            List<Comment> commentsList = new List<Comment>();

            foreach (var commentSyntax in singleLineComments)
            {
                if (commentSyntax.ToString().ToLower() == "//twcodeanalysis enable all" || commentSyntax.ToString().ToLower() == String.Format("//twcodeanalysis enable {0}", diagnosticId.ToLower()))
                {
                    commentsList.Add(new Comment(commentSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1, CommentType.OnComment));
                }
                else if (commentSyntax.ToString().ToLower() == "//twcodeanalysis disable all" || commentSyntax.ToString().ToLower() == String.Format("//twcodeanalysis disable {0}", diagnosticId.ToLower()))
                {
                    commentsList.Add(new Comment(commentSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1, CommentType.OffComment));
                }
            }


            int nodeLine = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
            Comment closestCommentBeforeNode = null;
            foreach (var comment in commentsList)
            {
                if (comment.Line < nodeLine)
                {
                    if (closestCommentBeforeNode == null) closestCommentBeforeNode = comment;
                    else if (comment.Line > closestCommentBeforeNode.Line) closestCommentBeforeNode = comment;
                }
            }

            if (closestCommentBeforeNode == null || closestCommentBeforeNode.Type == CommentType.OnComment) return false;
            else return true;

        }
    }

    internal class Comment
    {
        public int Line { get; private set; }
        public CommentType Type { get; private set; }

        public Comment(int line, CommentType type) {
            Line = line;
            Type = type;
        }

    }
    internal enum CommentType
    {
        OnComment, OffComment
    }
}
