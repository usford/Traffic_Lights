using System;

namespace Traffic_Lights.Info {
    //Элементы схемы в бд
    public class ElementInfoDB {
        public string? ID { get => _id; }
        private string? _id;
        public string? Name { get => _name; }
        private string? _name;
        public int? State { get => _state; }
        private int? _state;
        public string? Comment { get => _comment; }
        private string? _comment;

        public ElementInfoDB(string? id, string? name, int? state, string? comment) {
            _id = id;
            _name = name;
            _state = state;
            _comment = comment;
        }
    }
}
