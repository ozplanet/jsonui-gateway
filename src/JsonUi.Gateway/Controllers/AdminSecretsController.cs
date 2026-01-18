using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Gateway.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JsonUi.Gateway.Controllers;

[ApiController]
[Route("admin/secrets")]
[Authorize(Policy = "Admin")]
public sealed class AdminSecretsController : ControllerBase
{
    private readonly ISecretRepository _secrets;
    private readonly ISecretProtector _protector;

    public AdminSecretsController(ISecretRepository secrets, ISecretProtector protector)
    {
        _secrets = secrets;
        _protector = protector;
    }

    [HttpPost]
    public async Task<ActionResult<SecretResponse>> CreateAsync([FromBody] SecretCreateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Value))
        {
            return BadRequest(new { error = "Name and Value are required" });
        }

        var payload = _protector.Encrypt(request.Value);
        var secret = new Secret(request.Name);
        secret.UpdateValue(payload.Ciphertext, payload.Nonce, payload.Tag);

        await _secrets.AddAsync(secret, cancellationToken);

        return CreatedAtAction(nameof(GetAsync), new { id = secret.Id }, Map(secret));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SecretResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var secret = await _secrets.GetAsync(id, cancellationToken);
        if (secret is null)
        {
            return NotFound();
        }

        return Map(secret);
    }

    private static SecretResponse Map(Secret secret) => new(secret.Id, secret.Name, secret.CreatedAt);
}
