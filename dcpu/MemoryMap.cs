using System;
using System.Collections.Generic;

namespace Com.MattMcGill.Dcpu
{
	public class MemoryMap
	{
	    private class MappedRange {
	        public ushort Start { get; private set; }
	        
	        public ushort End { get; private set; }
	        
			public string Id { get; private set; }
	        
	        public MappedRange(ushort start, ushort end, string id) {
	            Start = start;
	            End = end;
	            Id = id;
	        }
	    }
	    
	    private List<MappedRange> _mappedRanges;
	    
	    public MemoryMap() {
	    	_mappedRanges = new List<MappedRange>();
		}
		
		public MemoryMap(MemoryMap map) {
			_mappedRanges = new List<MappedRange>();
			_mappedRanges.AddRange(map._mappedRanges);
		}
		
		public void Map(ushort from, ushort to, string id)
		{
            int i = 0;
            while (i < _mappedRanges.Count && _mappedRanges[i].End < from) ++i;

            if (i == _mappedRanges.Count) {
                _mappedRanges.Add(new MappedRange(from, to, id));
            } else if (_mappedRanges[i].Start <= to) {
                var msg = string.Format("Cannot map [{0}...{1}] to {2} -- [{3}...{4}] is already mapped to {5}!",
                    from, to, _mappedRanges[i].Start, _mappedRanges[i].End, _mappedRanges[i].Id);
                throw new ArgumentException(msg);
            } else {
                _mappedRanges.Insert(i, new MappedRange(from, to, id));
            }
		}
		
		public string this[ushort addr] {
			get {
	            foreach (var range in _mappedRanges) {
	                if (range.Start <= addr && addr <= range.End) {
	                	return range.Id;
	                }
	            }
	            return null;
			}
		}
	}
}

