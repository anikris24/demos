from authlib.jose import jwt
import json

# A secret key for signing the token (keep this secure!)
secret_key = 'ravi'

# The JWT header, specifying the algorithm
header = {'alg': 'HS256'}

# The JWT payload (the claims)
payload = {
    'iss': 'Indeed app',  # Issuer
    'sub': '67',           # Subject (user ID)
    'name': 'DK',
    'admin': True
}

# Encode the JWT
token_string = jwt.encode(header, payload, secret_key)
print("Encoded JWT:")
print(token_string.decode('utf-8'))

# Decode and verify the JWT
try:
    claims = jwt.decode(token_string, secret_key)
    # The decode function returns a JWTClaims object, which is like a dictionary
    print("\nDecoded JWT Claims:")
    print(json.dumps(claims, indent=2))
    
    # You can access claims like a dictionary
    print(f"User ID: {claims['sub']}")
    print(f"Is Admin: {claims['admin']}")

except Exception as e:
    print(f"Error decoding token: {e}")