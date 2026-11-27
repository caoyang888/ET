using System;
using System.IO;
using MemoryPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ET.SlotTests
{
    [TestClass]
    public class OuterFrameTests
    {
        [TestMethod]
        public void WebSocketOuterFrame_IsLittleEndianLengthPlusOpcodePlusMemoryPack()
        {
            C2G_EnterSlot message = new C2G_EnterSlot();
            message.RpcId = 7;
            byte[] payload = MemoryPackSerializer.Serialize(message);
            ushort opcode = Opcode.C2G_EnterSlot;
            byte[] body = new byte[2 + payload.Length];
            BitConverter.GetBytes(opcode).CopyTo(body, 0);
            Buffer.BlockCopy(payload, 0, body, 2, payload.Length);
            byte[] frame = new byte[2 + body.Length];
            BitConverter.GetBytes((ushort)body.Length).CopyTo(frame, 0);
            Buffer.BlockCopy(body, 0, frame, 2, body.Length);

            Assert.AreEqual(12005, opcode);
            Assert.AreEqual(0xE5, frame[2]);
            Assert.AreEqual(0x2E, frame[3]);
            ushort length = BitConverter.ToUInt16(frame, 0);
            Assert.AreEqual(body.Length, (int)length);
            C2G_EnterSlot decoded = MemoryPackSerializer.Deserialize<C2G_EnterSlot>(payload);
            Assert.AreEqual(7, decoded.RpcId);
            File.WriteAllText("/tmp/slot_enter_frame.hex", BitConverter.ToString(frame));
            File.WriteAllText("/tmp/slot_enter_payload.hex", BitConverter.ToString(payload));
        }

        [TestMethod]
        public void LoginString_UsesUtf8LengthComplement()
        {
            C2R_Login message = new C2R_Login();
            message.RpcId = 1;
            message.Account = "g";
            message.Password = string.Empty;
            byte[] payload = MemoryPackSerializer.Serialize(message);
            Assert.AreEqual("03-01-00-00-00-FE-FF-FF-FF-01-00-00-00-67-00-00-00-00", BitConverter.ToString(payload));
        }
    }
}
