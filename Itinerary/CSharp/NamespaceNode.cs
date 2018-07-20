namespace Itinerary.CSharp
{
    public class NamespaceNode
    : ICSharpNode
    {
        public NamespaceNode(string @namespace)
        {
            Namespace = @namespace;
        }

        public string GetLabel()
        {
            return "namespace " + Namespace;
        }

        public string Namespace { get; }
    }
}
