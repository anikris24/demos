from authlib.jose import JsonWebEncryption
# Use the private_key from the encryption example above

jwe = JsonWebEncryption()

# 1. The JWE token to decrypt
# Use the jwe_token from the previous example
jwe_token = None
jwe_token_bytes = None
with open("tok.jwe", "rb") as f:
    jwe_token_bytes = f.read()

# 2. Use the corresponding private key for decryption
# Use the private_key generated earlier
# private_key = """

# """
with open("pri.pem", "rb") as f:
    private_key = f.read()

# try:
#     private_key = RSAKey.from_pem(private_pem.encode('utf-8'))
#     print("Private key loaded successfully!")
#     # You can now use 'private_key' for signing or decryption
# except Exception as e:
#     print(f"Error loading key: {e}")

# 3. Decrypt the token
data = jwe.deserialize_compact(jwe_token_bytes, private_key)

# 4. Access the decrypted header and payload
decrypted_header = data['header']
decrypted_payload = data['payload']

print(f"Decrypted Header: {decrypted_header}")
print(f"Decrypted Payload: {decrypted_payload.decode()}")