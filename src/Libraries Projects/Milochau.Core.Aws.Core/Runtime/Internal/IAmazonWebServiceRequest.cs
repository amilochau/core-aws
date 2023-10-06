namespace Amazon.Runtime.Internal
{
    public interface IAmazonWebServiceRequest
    {
        SignatureVersion SignatureVersion { get; set; }
    }
}
