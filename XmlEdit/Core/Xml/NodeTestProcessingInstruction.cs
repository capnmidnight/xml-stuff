/*
 * 
 * AUTHOR: Sean T. McBeth, Sean T. McBeth
 * DATE: OCT-02-2007
 * 
 */

namespace XmlEdit
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A node test for matching a specific type of processing instruction. Processing instructions are blocks
    /// of text in an XML file that will be processed by an external program and replaced with valid XML. An example
    /// would be the <?php ... ?> block in PHP files.
    /// </summary>
    public class NodeTestProcessingInstruction : NodeTest
    {
        private string _parameter;
        internal NodeTestProcessingInstruction()
            : this("")
        {
        }
        public string Parameter
        {
            get
            {
                return this._parameter;
            }
        }
        public NodeTestProcessingInstruction(string parameter)
        {
            this._parameter = parameter;
        }
        public override string ToString()
        {
            return string.Format("processing-instruction({0})", this._parameter);
        }
    }

    internal class ProcessingInstructionNodeTestFactory : NodeTestFactory
    {
        public static NodeTestProcessingInstruction NoParameter;
        static ProcessingInstructionNodeTestFactory()
        {
            NoParameter = new NodeTestProcessingInstruction();
        }

        public bool Matches(string exp)
        {
            return exp.IndexOf("processing-instruction(") >= 0;
        }
        public NodeTest Create(string exp)
        {
            int start = exp.IndexOf('(');
            int end = exp.IndexOf(')');
            string param = exp.Substring(start + 1, end - start - 1);
            if (end > start + 1)
            {
                return new NodeTestProcessingInstruction(param);
            }
            else
            {
                return NoParameter;
            }
        }
    }
}
