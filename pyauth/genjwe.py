from authlib.jose import JsonWebEncryption
from cryptography.hazmat.primitives import serialization
from joserfc.jwk import RSAKey
import os, json

jwe = JsonWebEncryption()

# 1. Define the protected header
protected_header = {
    'alg': 'RSA-OAEP', # Key management algorithm
    'enc': 'A256GCM'   # Content encryption algorithm
}

# 2. Define the payload
# payload = b'This is a secret message.'
payload = {
    'iss': 'Indeed app',  # Issuer
    'sub': '67',           # Subject (user ID)
    'name': 'DK',
    'admin': True
}

# 3. Use an RSA public key for encryption
# In a real-world scenario, you would load this from a file or service.
# For this example, we'll generate a key pair.
private_keys = RSAKey.generate_key()
public_key = private_keys.public_key.public_bytes(encoding=serialization.Encoding.PEM, format=serialization.PublicFormat.SubjectPublicKeyInfo)
private_key = private_keys.private_key.private_bytes(encoding=serialization.Encoding.PEM, format=serialization.PrivateFormat.PKCS8, encryption_algorithm=serialization.NoEncryption())
# private_pem = key.as_pem()
# public_pem = key.as_pem()

with open("pri.pem", "wb") as f:
    f.write(private_key)

with open("pub.pem", "wb") as f:
    f.write(public_key)
    
# 4. Encrypt the data
jwe_token = jwe.serialize_compact(protected_header, json.dumps(payload).encode("utf-8"), private_keys.public_key)
print(f"JWE Token: {jwe_token.decode()}")

with open("tok.jwe", "wb") as f:
    f.write(jwe_token)

# print("Private Key (PEM):\n", private_pem.decode())
# print("\nPublic Key (PEM):\n", public_pem.decode())