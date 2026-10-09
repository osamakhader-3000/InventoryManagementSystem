using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationCore.Models
{
    public class RefreshToken(Guid id, Guid userId,string plainText, string hash, DateTime issuedAt, DateTime expiresAt, RefreshTokenStatus status,DateTime? rotatedAt, Guid familyId)
    {
        public Guid Id { get; private set; } = id;

        public Guid UserId { get; private set; } = userId;

        public string Plaintext { get; private set; } = plainText;

        public string Hash { get; private set; } = hash;


        public DateTime IssuedAt { get; private set; } = issuedAt;

        public DateTime ExpiresAt { get; private set; } = expiresAt;

        public DateTime? RotatedAt { get; private set; } = rotatedAt;

        public RefreshTokenStatus Status { get; private set; } = status;

        public Guid FamilyId { get; private set; } = familyId;
    }


    //status enums
    public enum RefreshTokenStatus
    {
        Current,
        Rotated,
        Revoked
    }

}
