using System.Collections.Frozen;
using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Feed.Services;
using Feed.Services.Abstractions;
using FeedData.Abstractions;
using Moq;
using NUnit.Framework.Legacy;
using Shared;

namespace FeedTests;


[TestFixture]
public class FeedServiceTests
{
    private Mock<IAesService> _aesMock;
    private Mock<IFeedData> _feedDataMock;

    private IFeedService _feedService;
    
    [SetUp]
    public void Setup()
    {
        _aesMock = new(MockBehavior.Strict);
        _feedDataMock  = new(MockBehavior.Strict);
        
        _feedService = new FeedService(_feedDataMock.Object,_aesMock.Object);
    }

    [Test]
    public async Task WriteData_ValidInput_CallsWriteDataOnFeedData()
    {
        //Arrange
        var platforms = new List<Platform>()
        { 
            new Platform("/ru","Ya"),
            new Platform("/ru/moscow","Vk")
        };
        
        var serializedPlatforms = JsonSerializer.Serialize(platforms);
        
        var encrypted = "encrypted";
        var iv = "IV";

        var serializedUpdateRequestBody = JsonSerializer.Serialize(new UpdateRequestBody(encrypted, iv));

        _aesMock.
            Setup(aes => aes.DecryptStringAsync(encrypted, iv)).
            ReturnsAsync(serializedPlatforms);

        _feedDataMock.Setup(data => data.WriteData(It.IsAny<FrozenDictionary<string, string[]>>()))
            .Callback<FrozenDictionary<string, string[]>>(dict =>
            {
                Assert.That(dict.ContainsKey("/ru"), Is.True);
                Assert.That(dict.ContainsKey("/ru/moscow"), Is.True);
                
                Assert.That(dict["/ru"], Is.EquivalentTo(new[] { "Ya" }));
                Assert.That(dict["/ru/moscow"], Is.EquivalentTo(new[] { "Ya", "Vk" }));
                
            }).Verifiable();
        
        //Act
        await _feedService.WriteData(serializedUpdateRequestBody);
        
        //Assert
        _feedDataMock.Verify(data => data.WriteData(It.IsAny<FrozenDictionary<string, string[]>>()), Times.Once);
    }

    [Test]
    public async Task WriteData_ValidInput_Twice_RewritesFirstInput()
    {
        //Arrange first dataset
        var firstPlatforms = new List<Platform>
        {
            new Platform("/ru", "Ya"), 
            new Platform("/ru/moscow", "Vk")
        };
        
        var serializedFirstPlatforms = JsonSerializer.Serialize(firstPlatforms);
        
        var encryptedFirst = "encrypted1";
        var ivFirst = "IV1";
        
        var serializedFirstRequest = JsonSerializer.Serialize(new UpdateRequestBody(encryptedFirst, ivFirst));

        //Arrange second dataset
        var secondPlatforms = new List<Platform>
        {
            new Platform("/ru/spb", "idk"), 
            new Platform("/ru/sochi", "idk2")
        };
        
        var serializedSecondPlatforms = JsonSerializer.Serialize(secondPlatforms);
        
        var encryptedSecond = "encrypted2";
        var ivSecond = "IV2";
        
        var serializedSecondRequest = JsonSerializer.Serialize(new UpdateRequestBody(encryptedSecond, ivSecond));
        
        int callCount = 0;
        
        _aesMock.Setup(aes => aes.DecryptStringAsync(encryptedFirst, ivFirst)).
            ReturnsAsync(serializedFirstPlatforms);
        
        _aesMock.Setup(aes => aes.DecryptStringAsync(encryptedSecond, ivSecond)).
            ReturnsAsync(serializedSecondPlatforms);
        
        _feedDataMock.Setup(data => data.WriteData(It.IsAny<FrozenDictionary<string, string[]>>()))
            .Callback<FrozenDictionary<string, string[]>>(dict =>
            {
                callCount++;
                
                if (callCount == 2)
                {
                    Assert.That(dict.ContainsKey("/ru"), Is.False);
                    Assert.That(dict.ContainsKey("/ru/moscow"), Is.False);
                }
                
            }).Verifiable();

        //Act
        await _feedService.WriteData(serializedFirstRequest);
        await _feedService.WriteData(serializedSecondRequest);
        
        //Assert
        _feedDataMock.Verify(data => data.WriteData(It.IsAny<FrozenDictionary<string, string[]>>()), Times.Exactly(2));
    }

    [Test]
    public async Task GetPlatforms_ValidInput_CallsGetPlatformsOnFeedData_GetsCachedThen()
    {
        //Arrange
        string query = "/ru";

        var platforms = new []
        {  
            "Ya"
        };

        var serializedPlatforms = JsonSerializer.Serialize
            (
                new List<string>()
                {
                    "Ya"
                }
            );

        var encryted = "encrypted";
        var iv = "IV";
        
        _feedDataMock.
            Setup(data => data.GetPlatforms(query)).
            Returns(platforms);

        _aesMock.
            Setup(aes => aes.EncryptStringAsync(serializedPlatforms)).
            ReturnsAsync((encryted, iv));
        
        //Act
        var result = await _feedService.GetPlatforms(query);
        var result2 = await _feedService.GetPlatforms(query);


        //Assert
        Assert.That(result, Is.Not.Default);
        Assert.That(result2, Is.Not.Default);
        
        
        Assert.That(result, Is.EqualTo(result2));

        Assert.That(result.EncryptedData, Is.EqualTo(encryted));
        Assert.That(result.Iv, Is.EqualTo(iv));
        
        
        _aesMock.Verify(aes => aes.EncryptStringAsync(It.IsAny<string>()),Times.Once);
    }

    [Test]
    public async Task GetPlatforms_ResultForInputDoesNotExist_ReturnsDefault()
    {
        //Assert
        var query = "/ru/moscow";
        
        _feedDataMock.
            Setup(data => data.GetPlatforms(query)).
            Returns(() => null);
        
        //Act
        var result = await _feedService.GetPlatforms(query);
        
        //Assert
        Assert.That(result, Is.Default);
    }
    

}