//
// Copyright (c) 2018 Canti, The TurtleCoin Developers
// 
// Please see the included LICENSE file for more information.

using System;

namespace Canti.Blockchain
{
    public class IBlockchainCache
    {
        IBlockchainCache parent { get; set; }
        uint BlockIndex { get; set; }
        uint GlobalOutputIndex { get; set; }
        ulong Amount { get; set; }
        string[] chainLeaves { get; set; }

        public bool hasBlock(string blockHash)
        {
            return findSegmentContainingBlock(blockHash) != null;
        }

        public IBlockchainCache findIndexInChain(IBlockchainCache blockSegment, string blockHash)
        {
            if (blockSegment == null) throw new Exception();

            while (blockSegment != null)
            {
                if (blockSegment.hasBlock(blockHash)) return blockSegment;
                blockSegment = blockSegment.getParent();
            }
            return null;
        }

        public IBlockchainCache findSegmentContainingBlock(string blockHash)
        {
            if (Globals.DAEMON_CHAIN_LEAVES.Length < 1) throw new Exception("Chain leaves must be greater than 0");

            // first search in main chain
            IBlockchainCache blockSegment = findIndexInChain(Globals.DAEMON_CHAIN_LEAVES[0], blockHash);
            if (blockSegment != null) return blockSegment;

            // than search in alternative chains
            return findAlternativeSegmentContainingBlock(blockHash);
        }

        IBlockchainCache findAlternativeSegmentContainingBlock(string blockHash)
        {
            IBlockchainCache cache = null;
            /*std::find_if(++chainsLeaves.begin(), chainsLeaves.end(),
            [&] (IBlockchainCache* chain) { return cache = findIndexInChain(chain, blockHash);
            });*/
            return cache;
        }

        IBlockchainCache getParent()
        {
            return parent;
        }
    }
}
