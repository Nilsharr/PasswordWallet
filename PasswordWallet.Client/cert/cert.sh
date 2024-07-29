subj='/C=PL/O=Password Wallet/CN=localhost'
if [[ "$(uname -sr)" =~ ^(CYGWIN|MINGW|MINGW32|MSYS).* ]]; then
    subj='//C=PL\O=Password Wallet\CN=localhost'
fi

mkdir -p ./local

openssl req -x509 -nodes -new -sha512 \
  -days 730 -newkey rsa:4096 -keyout ca.key \
  -out ca.pem -subj "$subj"
  
openssl x509 -outform pem -in ca.pem -out ca.crt

openssl req -new -nodes -newkey rsa:4096 \
  -keyout ./local/localhost.key -out ./local/localhost.csr \
  -subj "$subj"
  
openssl x509 -req -sha512 -days 730 \
  -extfile v3.ext \
  -CA ca.crt -CAkey ca.key -CAcreateserial \
  -in ./local/localhost.csr \
  -out ./local/localhost.crt