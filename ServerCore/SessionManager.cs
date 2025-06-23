// ServerCore 프로젝트에 포함
// SessionManager.cs
using System;
using System.Collections.Generic;
using ServerCore;
using System.Threading; // Interlocked 사용을 위해

public class SessionManager
{
    public static SessionManager Instance { get; } = new SessionManager(); // 싱글톤

    List<Session> _sessions = new List<Session>(); // 접속 중인 세션 목록
    object _lock = new object(); // _sessions 리스트 보호를 위한 Lock
    long _newSessionId = 0; // 세션에 고유 ID 부여를 위한 카운터 (Interlocked 사용)

    private SessionManager() { } // 외부에서 인스턴스화 방지

    // 새로운 세션이 연결될 때 호출
    public void Add(Session session)
    {
        lock (_lock) // Critical Section (임계 영역)
        {
            // 고유 ID 부여 (Interlocked를 사용하여 스레드 안전하게 증가)
            Interlocked.Increment(ref _newSessionId);
            session.SessionId = _newSessionId; // Session 클래스에 SessionId 속성 추가 필요

            _sessions.Add(session);
            Console.WriteLine($"[SessionManager] Session {session.SessionId} connected. Total sessions: {_sessions.Count}");
        }
    }

    // 세션이 끊길 때 호출
    public void Remove(Session session)
    {
        lock (_lock) // Critical Section
        {
            _sessions.Remove(session);
            Console.WriteLine($"[SessionManager] Session {session.SessionId} disconnected. Total sessions: {_sessions.Count}");
        }
    }

    // 모든 세션에게 패킷 브로드캐스트
    public void Broadcast(ArraySegment<byte> packet)
    {
        lock (_lock)
        {
            List<Session> toRemove = new();

            foreach (Session session in _sessions)
            {
                try
                {
                    session.Send(packet);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Broadcast Error] Session {session.SessionId}: {ex.Message}");
                    toRemove.Add(session); // 실패한 세션은 제거 대상
                }
            }

            // 실패한 세션 제거
            foreach (var session in toRemove)
            {
                _sessions.Remove(session);
            }
        }
    }

    // 모든 세션 리스트를 가져오는 메서드 (필요 시)
    public List<Session> GetSessions()
    {
        lock (_lock)
        {
            return new List<Session>(_sessions); // _sessions 리스트의 복사본 반환 (외부 수정 방지)
        }
    }

    public ClientSession FindByUserId(string userId)
    {
        lock (_lock)
        {
            foreach (Session s in _sessions)
            {
                if (s is ClientSession cs && cs.UserId == userId)
                    return cs;
            }
        }
        return null;
    }

    public void BroadcastUserList()
    {
        List<string> userIds = new List<string>();
        lock (_lock)
        {
            foreach (Session session in _sessions)
            {
                if (session is ClientSession cs && cs.UserId != null)
                    userIds.Add(cs.UserId);
            }
        }

        ServerUserListPacket packet = new ServerUserListPacket { UserIds = userIds };
        ArraySegment<byte> sendBuffer = packet.ToBytes();

        Broadcast(sendBuffer);
    }
}