using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PureGym.SharedKernel.Events;

namespace PureGym.Mock.RevolvingDoor;

[ApiController]
[Route("api/door")]
public class DoorController : ControllerBase
{
    private readonly DoorState _state;
    private readonly IPublishEndpoint _publishEndpoint;

    public DoorController(DoorState state, IPublishEndpoint publishEndpoint)
    {
        _state = state;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        var user = _state.CurrentUser;
        if (user == null) return Ok("The door is empty.");

        return Ok(new { Message = "User waiting", User = user });
    }

    // The Big Green Button
    [HttpPost("enter")]
    public async Task<IActionResult> LetThemIn()
    {
        var user = _state.CurrentUser;
        if (user == null) return BadRequest("Nobody is waiting at the door!");

        await _publishEndpoint.Publish(new GymEnteredEvent(user.MemberId, DateTime.UtcNow));

        _state.Clear();

        return Ok("User passed through. Door is now closed.");
    }

    [HttpPost("cancel")]
    public IActionResult CancelEntry()
    {
        if (_state.CurrentUser == null) return BadRequest("Nobody to cancel.");

        _state.Clear();
        return Ok("User walked away. Door cleared.");
    }
}
