```diff
- This document is under contruction!!!
```
# Wasabi Remote Procedure Call interface

Wasabi wallet provides an RPC interface to interact with the wallet programatically. The RPC server is listing by default on port 18099. 

## Limitations

The RPC server does NOT support batch requests, authentication nor SSL/TLS communication. Requests are served in order one by one in serie (no parallel processing).
It is intentionally limited to serve to only one whitelisted local address and comes disabled by default.


# Methods available
Current version only handles the following method `listunspentcoins`, `getstatus`, `getwalletinfo`, `getnewaddress` and `stop`. These can can be tested as follow:

## getstatus

Returns information useful to understand the real status wasabi and its synchronization status.

```bash
curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"getstatus"}' http://127.0.0.1:18099/ 
```
```json
{
  "jsonrpc": "2.0",
  "result": {
    "torStatus": "Running",
    "backendStatus": "Connected",
    "bestBlockchainHeight": "1517613",
    "bestBlockchainHash": "0000000000000064db138798b6b789910bc7f29546a1ff506734dc7bb5780b
28",
    "filtersCount": 689039,
    "filtersLeft": 0,
    "network": "TestNet",
    "exchangeRate": 7822.24,
    "peers": [
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:20.0452963+00:00",
        "endpoint": "[::ffff:178.128.20.150]:18333",
        "userAgent": "/Satoshi:0.17.1(bitcore)/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:40.5267439+00:00",
        "endpoint": "[::ffff:142.93.231.134]:18333",
        "userAgent": "/Satoshi:0.17.0/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:35.6408702+00:00",
        "endpoint": "[::ffff:47.74.4.44]:18884",
        "userAgent": "/Satoshi:0.13.2/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:44.371711+00:00",
        "endpoint": "[fd87:d87e:eb43:7081:2a4e:edaa:f8bb:10d0]:18333",
        "userAgent": "/Satoshi:0.16.2/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:05.6071169+00:00",
        "endpoint": "[::ffff:153.126.145.243]:18333",
        "userAgent": "/Satoshi:0.15.1/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:35.3028356+00:00",
        "endpoint": "[::ffff:46.4.171.125]:18333",
        "userAgent": "/Satoshi:0.16.0/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:36.1277937+00:00",
        "endpoint": "[::ffff:52.213.124.148]:18333",
        "userAgent": "/Satoshi:0.17.1/"
      },
      {
        "isConnected": true,
        "lastSeen": "2019-05-20T19:35:35.6409197+00:00",
        "endpoint": "[::ffff:35.241.203.98]:18333",
        "userAgent": "/Satoshi:0.15.2/"
      }
    ]
  },
  "id": "1"
}
```

## listunspentcoins

Returns the list of confirmed and unconfirmed coins that are unspent.

```bash
$ curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"listunspentcoins"}' http://127.0.0.1:18099/
```
```json
{
  "jsonrpc": "2.0",
  "result": [
    {
      "txid": "7958defd85bb4f34dc66b3323eaa2f4fbdbece35f5e1b2d7b49b461ddc2066ec",
      "index": 0,
      "amount": 109859,
      "anonymitySet": 1,
      "confirmed": true,
      "label": "sss",
      "keyPath": "84'/0'/0'/0/616",
      "address": "tb1qgcng6v7wt03t80x6gh7s2x4rawg9zhenzrek4y"
    },
    {
      "txid": "06482d847623e096394ecfae58e86e075b7761da493e6eee45a6f2f9c8909582",
      "index": 0,
      "amount": 2994272,
      "anonymitySet": 9,
      "confirmed": true,
      "label": "",
      "keyPath": "84'/0'/0'/1/79",
      "address": "tb1qgfcv3pgj6tvzc5g73l7tps58q30zx8qk3y35uu"
    },
    {
      "txid": "aaddc190fe0c2612559b28f9a4a6f4e78906e1794545badccd2fc318257fe2c4",
      "index": 0,
      "amount": 195218,
      "anonymitySet": 1,
      "confirmed": true,
      "label": "hola loco",
      "keyPath": "84'/0'/0'/0/623",
      "address": "tb1q2dgj9u3ggjg08hvvhf3l4m3u3ncpdxud8m0yqu"
    }
  ],
  "id": "1"
}
```  

In case there is no wallet opened it will return:
```json
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32603,
    "message": "There is not wallet loaded."
  },
  "id": "1"
}
```

## getwalletinfo

Returns information about the current loaded wallet (this method could be extended to return info about any other wallet)

```bash
curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"getwalletinfo"}' http://127.0.0.1:18099/ 
```
```json
{
  "jsonrpc": "2.0",
  "result": {
    "walletFile": "/home/user/.walletwasabi/client/Wallets/testnet-wallet.json",
    "extendedAccountPublicKey": "tpubDCd1v6acjNY3uUqAtBGC6oBTGrCBWphMvkWjAqM2SFZahZb91JUT
XZeZqxzscezR16XHkwi1723qo94EKgR75aoFaahnaHiiLP2JrrTh2Rk",
    "extendedAccountZpub": "vpub5YarnXR6ijVdw6G5mGhrUhf5bnodeCDJYtszFVW7LL3vr5HyRmJF8zfTZ
Wzv6LjLPukmeR11ebWhLPLVVRjqbfyknJZdiwRWCyJcKeDdsC8",
    "accountKeyPath": "m/84'/0'/0'",
    "masterKeyFingerprint": "323ec8d9"
  },
  "id": "1"
}
```
In case there is no wallet opened it will return:
```json
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32603,
    "message": "There is not wallet loaded."
  },
  "id": "1"
}
```

## getnewaddress

Creates and returns info about the created address.

```bash
curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"getnewaddress","params":["payment order #178659"]}' http://127.0.0.1:18099/ 
```
```json
{
  "jsonrpc": "2.0",
  "result": {
    "address": "tb1qdskc4y529ayqkqrddknnhdjqwnqc9wzl8940pn",
    "keyPath": "84'/0'/0'/0/30",
    "label": "payment order #178659",
    "publicKey": "0263ea6712e56277bcb07b14b61c30bae2267ec10e0bbf7a024d2c6a0634d6e634",
    "p2wpkh": "00146c2d8a928a2f480b006d6da73bb64074c182b85f"
  },
  "id": "1"
}
```

In case there is no wallet opened it will return:
```json
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32603,
    "message": "There is not wallet loaded."
  },
  "id": "1"
}
```

In case an empty label is provided:
```
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32603,
    "message": "A non-empty label is required."
  },
  "id": "1"
}
```

## Stop

Stops and exit Wasabi.

```bash
curl -s --data-binary '{"jsonrpc":"2.0", "method":"stop"}' http://127.0.0.1:18099/ 
```

------

## Errors

### Method not found
```bash
$ curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"howknows"}' http://127.0.0.1:18099/
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32601,
    "message": "howknows method not found."
  },
  "id": "1"
}
```

### Parse error
```bash
$ curl -s --data-binary '{"jsonrpc":"2.0" []}' http://127.0.0.1:18099/
```
```json
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32700,
    "message": "Parse error"
  }
}
```

### Mismatching parameters
```bash
$ curl -s --data-binary '{"jsonrpc":"2.0", "method": "getnewaddress", "params": { "lable": "label with a type" }, "id":"1" }' http://127.0.0.1:18099/
```

```json
{
  "jsonrpc": "2.0",
  "error": {
    "code": -32602,
    "message": "A value for the 'label' is missing."
  },
  "id": "1"
}
```
