// Copyright 2012-2013 The CryptoNote Developers
// Copyright 2014-2018 The Monero Developers
// Copyright 2018 The TurtleCoin Developers
//
// Please see the included LICENSE file for more information.

using System;

using Canti.Data;

namespace Canti.Blockchain.Crypto.Keccak
{
    internal static class Keccak
    {
        private static void Keccakf(ulong[] state, int rounds)
        {
            ulong t;

            ulong[] bc = new ulong[5];

            for (int round = 0; round < rounds; round++)
            {
                /* Theta */
                for (int i = 0; i < 5; i++)
                {
                    bc[i] = state[i] ^ state[i+5] ^ state[i+10] ^ state[i+15] ^ state[i + 20];
                }

                for (int i = 0; i < 5; i++)
                {
                    t = bc[(i + 4) % 5] ^ ROTL64(bc[(i + 1) % 5], 1);

                    for (int j = 0; j < 25; j += 5)
                    {
                        state[i + j] ^= t;
                    }
                }

                /* Rho Pi */
                t = state[1];

                for (int i = 0; i < 24; i++)
                {
                    int j = Constants.keccakf_piln[i];
                    bc[0] = state[j];
                    state[j] = ROTL64(t, (ulong)(Constants.keccakf_rotc[i]));
                    t = bc[0];
                }

                /* Chi */
                for (int j = 0; j < 25; j += 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        bc[i] = state[i + j];
                    }

                    for (int i = 0; i < 5; i++)
                    {
                        state[i + j] ^= (~bc[(i + 1) % 5]) & bc[(i + 2) % 5];
                    }
                }

                /* Iota */
                state[0] ^= Constants.keccakf_rndc[round];
            }
        }

        public static byte[] keccak(byte[] input, int outputSize = 32)
        {
            ulong[] state = new ulong[25];

            int rsiz;

            if (state.Length == outputSize)
            {
                rsiz = Constants.HASH_DATA_AREA;
            }
            else
            {
                rsiz = 200 - 2 * outputSize;
            }

            int inputLength = input.Length;

            int offset = 0;

            /* We index the array 8 bytes at a time, and treat each set of
               bytes as a long */
            for (; inputLength >= rsiz; inputLength -= rsiz, offset += rsiz * 8)
            {
                for (int i = 0; i < rsiz; i += 8)
                {
                    /* Read 8 bytes as a ulong */
                    state[i / 8] ^= Encoding.ByteArrayToInteger<ulong>(input, offset + i, 8);
                }

                Keccakf(state, Constants.KECCAK_ROUNDS);
            }

            byte[] temp = new byte[144];

            for (int i = 0; i < inputLength; i++)
            {
                temp[i] = input[i + offset];
            }

            temp[inputLength++] = 1;

            for (int i = inputLength; i < rsiz - inputLength; i++)
            {
                temp[i] = 0;
            }

            temp[rsiz - 1] |= 0x80;

            /* We proceed in chunks of 8 bytes as once */
            for (int i = 0; i < rsiz; i += 8)
            {
                /* Read 8 bytes as a ulong */
                state[i / 8] ^= Encoding.ByteArrayToInteger<ulong>(temp, i, 8);
            }

            Keccakf(state, Constants.KECCAK_ROUNDS);

            byte[] output = new byte[outputSize];

            /* Copy the ulong state into the output array as bytes */
            Buffer.BlockCopy(state, 0, output, 0, output.Length);

            return output;
        }

        private static ulong ROTL64(ulong x, ulong y)
        {
            return x << (int)y | x >> (int)((64 - y));
        }
    }
}
