# Wasabi Remote Procedure Call interface

Wasabi wallet provides an RPC interface to interact with the wallet programatically. The RPC server is listing by default on port 18099. 

## Limitations

The RPC server does NOT support batch requests nor TLS communication (because it is not supported on Linux nor Mac: https://github.com/dotnet/corefx/issues/14691). Requests are served in order one by one in serie (no parallel processing).
It is intentionally limited to serve to only one whitelisted local address and comes disabled by default.

# Before start (Enable RPC)

The RPC server has to be configured and enabled. This is done in the `Config.json` file and the relevant settings are:

* JsonRpcServerEnabled: [true | false]  (default: false)
* JsonRpcServerPrefixes: [an array of string with prefixes]
	(default: [	"http://127.0.0.1:18099/", "http://localhost:18099/"])

# Authentication

The RPC server can be configured to allow `Anonymous` access or `Basic  authentication` just by editing the 

* JsonRpcUser: [the username]  (default: empty)
* JsonRpcPassword: [the password]  (default: empty)

By default both `JsonRpcUser` and `JsonRpcPassword` are empty (`""`) that that means that `Anonymous` requests are allowd. On the other hand, if `JsonRpcUser` and `JsonRpcPassword` setting are not empty it means that the requester has to provide the right credential, otherwise it will get a http statuc code 401 (Unauthorized).


# 

# Methods available

Current version only handles the following method `listunspentcoins`, `getstatus`, `getwalletinfo`, `getnewaddress`, `send` and `stop`. These can can be tested as follow:

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

## send

Build and broadcast a transaction

```bash
curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"send", "params": { "sendto": "tb1qjlls57n6kgrc6du7yx4da9utdsdaewjg339ang", "coins":[{"transactionid":"8c5ef6e0f10c68dacd548bbbcd9115b322891e27f741eb42c83ed982861ee121", "index":0}], "amount": 15000, "label": "test transaction", "feeTarget":2 }}' http://127.0.0.1:18099/
```
```json
{"jsonrpc":"2.0","result": {"txid":"c493bf3d9e0279968bf677f0b3661f8f67823b6d7524b9c8a278701d0fb357a5","tx":"0100000000010121e11e8682d93ec842eb41f7271e8922b31591cdbb8b54cdda680cf1e0f65e8c0000000000ffffffff0247590000000000001600143cc0c9d8649532ad77fe0ac5032c1c9ad9529109983a00000000000016001497ff0a7a7ab2078d379e21aade978b6c1bdcba480247304402206bdbf36d0be8062e69b22441ed496ce6b5d639663bd4e580d11b71e34d79c6760220375219a8eb695f15a992b502ff4b639ea809547a3b7f0596cf9a69c1ae1d0df60121033e8670324ec33f15dcb17f346c1927ee3b717070596e397eb00020899c9c913300000000"}}
```

In case of error it is reported in the error:
```bash
 curl -s --data-binary '{"jsonrpc":"2.0","id":"1","method":"send", "params": { "sendto": "tb1qnmfmkylkd548bbbcd9115b322891e27f741eb42c83ed982861ee121", "coins":[{"transactionid":"c68dacd548bbbcd9115b38ed982861ee121c5ef6e0f1022891e27f741eb42c83", "index":0}], "amount": 2015663, "label": "test", "feeTarget": 2 }}' http://127.0.0.1:18099/
```
```json
{"jsonrpc":"2.0","error":{"code":-32603,"message":"Needed: 0.02015663 BTC, got only: 0.001 BTC."},"id":"1"}
```

**Note**: error codes are generic and not wasabi specific.

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
