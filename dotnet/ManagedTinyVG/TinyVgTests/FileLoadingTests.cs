using NUnit.Framework;
using TinyVgCore;
#pragma warning disable CS8602

namespace TinyVgTests;

[TestFixture]
public class FileLoadingTests
{
    
    [Test]
    public void corrupted_files_are_rejected()
    {
        var byteArray = File.ReadAllBytes("examples/corrupted.tvg");

        var exception = Assert.Throws<Exception>(() => { TvgLoad.FromByteArray(byteArray); });
        Assert.That(exception.Message, Contains.Substring("not a valid TinyVg document"));
    }

    [Test]
    public void can_load_a_file_from_byte_array()
    {
        var byteArray = File.ReadAllBytes("examples/everything-1.tvg");
        
        var document = TvgLoad.FromByteArray(byteArray);
        
        Assert.That(document, Is.Not.Null);
    }
}