using System.Text.Json;
using AesService.Abstractions;
using Domain;
using Moq;
using PortManager.Abstractions;
using Sender.Services;
using Sender.Services.Abstractions;
using Shared;

namespace SenderTests;

[TestFixture]
public class SenderServiceTests
{
    private Mock<IAesService> _aesMock;
    private Mock<IPortManager> _portMock;

    private ISenderService _senderService;
    
    [SetUp]
    public void Setup()
    {
        _aesMock = new(MockBehavior.Strict);
        _portMock = new(MockBehavior.Strict);

        _senderService = new SenderService(_portMock.Object, _aesMock.Object);
    }

    [Test]
    public async Task SendPlatformListAsync_ValidInput_CallsOnPortManager()
    {
        //Arrange
        var serializedPlatforms = JsonSerializer.Serialize(new List<Platform>
        {
            new Platform("/ru","Ya")
        });

        var encrypted = "encrypted";
        var iv = "IV";

        var serializedBody = JsonSerializer.Serialize(new UpdateRequestBody(encrypted, iv));
        
        _aesMock.
            Setup(aes => aes.EncryptStringAsync(serializedPlatforms)).
            ReturnsAsync((encrypted,iv));
        
        _portMock.
            Setup(port => port.SendPlatformListAsync(serializedBody)).
            Returns(Task.CompletedTask);
        
        //Act
        await _senderService.SendPlatformListAsync(serializedPlatforms);
        
        //Assert
        _portMock.Verify(port => port.SendPlatformListAsync(serializedBody),Times.Once);
    }

    [Test]
    public async Task SendPlatformListAsync_InvalidInput_DoesNotThrow()
    {
        Assert.DoesNotThrowAsync( async () => await _senderService.SendPlatformListAsync("hui"));
    }
}