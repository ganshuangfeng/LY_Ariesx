// Code generated by protoc-gen-go. DO NOT EDIT.
// source: static_map.proto

package protocol

import proto "github.com/golang/protobuf/proto"
import fmt "fmt"
import math "math"

// Reference imports to suppress errors if they are not otherwise used.
var _ = proto.Marshal
var _ = fmt.Errorf
var _ = math.Inf

type StaticMap struct {
	Points           []*Point `protobuf:"bytes,1,rep,name=Points" json:"Points,omitempty"`
	XXX_unrecognized []byte   `json:"-"`
}

func (m *StaticMap) Reset()                    { *m = StaticMap{} }
func (m *StaticMap) String() string            { return proto.CompactTextString(m) }
func (*StaticMap) ProtoMessage()               {}
func (*StaticMap) Descriptor() ([]byte, []int) { return fileDescriptor8, []int{0} }

func (m *StaticMap) GetPoints() []*Point {
	if m != nil {
		return m.Points
	}
	return nil
}

func init() {
	proto.RegisterType((*StaticMap)(nil), "protocol.StaticMap")
}

func init() { proto.RegisterFile("static_map.proto", fileDescriptor8) }

var fileDescriptor8 = []byte{
	// 95 bytes of a gzipped FileDescriptorProto
	0x1f, 0x8b, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0xff, 0xe2, 0x12, 0x28, 0x2e, 0x49, 0x2c,
	0xc9, 0x4c, 0x8e, 0xcf, 0x4d, 0x2c, 0xd0, 0x2b, 0x28, 0xca, 0x2f, 0xc9, 0x17, 0xe2, 0x00, 0x53,
	0xc9, 0xf9, 0x39, 0x52, 0x3c, 0xc5, 0x25, 0x45, 0xa5, 0xc9, 0x25, 0x10, 0x71, 0x25, 0x13, 0x2e,
	0xce, 0x60, 0xb0, 0x5a, 0xdf, 0xc4, 0x02, 0x21, 0x75, 0x2e, 0xb6, 0x80, 0xfc, 0xcc, 0xbc, 0x92,
	0x62, 0x09, 0x46, 0x05, 0x66, 0x0d, 0x6e, 0x23, 0x7e, 0x3d, 0x98, 0x2e, 0x3d, 0xb0, 0x78, 0x10,
	0x54, 0x1a, 0x10, 0x00, 0x00, 0xff, 0xff, 0xb2, 0x00, 0xd0, 0x32, 0x60, 0x00, 0x00, 0x00,
}
