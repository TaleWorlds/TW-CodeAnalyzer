﻿using Microsoft.CodeAnalysis;
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
        public static AnalyzerDisablingComments Instance 
        { 
            get 
            {
                if (_instance == null)
                {
                    _instance = new AnalyzerDisablingComments();
                }
                return _instance;
            }
        }

        public bool IsInDisablingComments(SyntaxNode node, String diagnosticId) //TODO: enable/disable this line
        {
            var root = node.SyntaxTree.GetRoot();
            var singleLineComments = root.DescendantTrivia().Where(trivia => trivia.IsKind(SyntaxKind.SingleLineCommentTrivia));

            List<_comment> commentsList = new List<_comment>();
            bool disabled = false;
            foreach (var commentSyntax in singleLineComments)
            {
                if (commentSyntax.ToString().ToLower() == "//twcodeanalysis enable all" || commentSyntax.ToString().ToLower() == String.Format("//twcodeanalysis enable {0}", diagnosticId.ToLower()))
                {
                    commentsList.Add(new _comment(commentSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1, CommentType.OnComment));
                }
                else if (commentSyntax.ToString().ToLower() == "//twcodeanalysis disable all" || commentSyntax.ToString().ToLower() == String.Format("//twcodeanalysis disable {0}", diagnosticId.ToLower()))
                {
                    commentsList.Add(new _comment(commentSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1, CommentType.OffComment));
                }
                else if (commentSyntax.ToString().ToLower() == "//twcodeanalysis disable next line all" || commentSyntax.ToString().ToLower() == String.Format("//twcodeanalysis disable next line {0}", diagnosticId.ToLower()))
                {
                    
                    if (commentSyntax.GetLocation().GetLineSpan().StartLinePosition.Line + 1 == node.GetLocation().GetLineSpan().StartLinePosition.Line)
                    {
                        disabled = true;
                    }
                }
            }

            if (!disabled)
            {
                int nodeLine = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
                _comment closestCommentBeforeNode = null;
                foreach (var comment in commentsList)
                {
                    if (comment.Type == CommentType.OffComment)
                    {
                        continue;
                    }

                    if (comment.Line < nodeLine)
                    {
                        if (closestCommentBeforeNode == null)
                        {
                            closestCommentBeforeNode = comment;
                        }
                        else if (comment.Line > closestCommentBeforeNode.Line)
                        {
                            closestCommentBeforeNode = comment;
                        }
                    }
                }

                if (closestCommentBeforeNode == null || closestCommentBeforeNode.Type == CommentType.OnComment)
                {
                    disabled = false;
                }
                else
                {
                    disabled = true;
                }
            }

            return disabled;
        }

        private class _comment
        {
            public int Line 
            { 
                get; 
                private set; 
            }
            public CommentType Type 
            { 
                get; 
                private set; 
            }

            public _comment(int line, CommentType type)
            {
                Line = line;
                Type = type;
            }

        }
        private enum CommentType
        {
            OnComment, 
            OffComment,
            SingleOffComment
        }
    }

    
}
