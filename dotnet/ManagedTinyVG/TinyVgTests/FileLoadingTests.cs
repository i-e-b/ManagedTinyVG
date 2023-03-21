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
        Assert.That(exception.Message, Contains.Substring("Not a valid TinyVg document"));
    }

    [Test]
    public void can_load_a_file_from_byte_array()
    {
        var byteArray = File.ReadAllBytes("examples/everything-1.tvg"); //400 x 768
        
        var document = TvgLoad.FromByteArray(byteArray);
        
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Height, Is.EqualTo(768), "height");
        Assert.That(document.Width, Is.EqualTo(400), "width");
    }
    
    [Test]
    public void can_load_a_file_from_a_file_stream()
    {
        using var fs = File.Open("examples/everything-1.tvg", FileMode.Open); //400 x 768
        
        var document = TvgLoad.FromStream(fs);
        
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Height, Is.EqualTo(768), "height");
        Assert.That(document.Width, Is.EqualTo(400), "width");
    }
    
    [Test]
    public void load_everything_32()
    {
        var byteArray = File.ReadAllBytes("examples/everything-32.tvg"); //400 x 768
        
        var document = TvgLoad.FromByteArray(byteArray);
        
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Height, Is.EqualTo(768), "height");
        Assert.That(document.Width, Is.EqualTo(400), "width");
    }
    
    [Test]
    public void load_shield_8()
    {
        var byteArray = File.ReadAllBytes("examples/shield-8.tvg");
        
        var document = TvgLoad.FromByteArray(byteArray);
        
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Height, Is.EqualTo(24), "height");
        Assert.That(document.Width, Is.EqualTo(24), "width");
    }
    
    [Test]
    public void load_shield_16()
    {
        var byteArray = File.ReadAllBytes("examples/shield-16.tvg");
        
        var document = TvgLoad.FromByteArray(byteArray);
        
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Height, Is.EqualTo(24), "height");
        Assert.That(document.Width, Is.EqualTo(24), "width");
    }
    
    [Test]
    public void load_shield_32()
    {
        var byteArray = File.ReadAllBytes("examples/shield-32.tvg");
        
        var document = TvgLoad.FromByteArray(byteArray);
        
        Assert.That(document, Is.Not.Null);
        Assert.That(document.Height, Is.EqualTo(24), "height");
        Assert.That(document.Width, Is.EqualTo(24), "width");
    }
}