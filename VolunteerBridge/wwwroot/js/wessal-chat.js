// wessal-chat.js
// Handles the global floating chat widget state and SignalR connection

window.WessalChat = (function() {
    // DOM Elements
    let container, btn, windowEl, badge;
    let inboxView, threadView;
    let inboxList, messagesArea, form, input, threadName, threadContext, backBtn, backBadge;
    let emptyTemplate;

    // State
    let isOpen = false;
    let currentView = 'inbox'; // 'inbox' or 'thread'
    let activeThreadUserId = null;
    let connection = null;
    let currentUserId = null;
    let hasLoadedInbox = false;
    let lastRenderedDate = null; // Track day boundaries in threads

    const arabicFormatter = {
        time: new Intl.DateTimeFormat('ar-EG', { hour: 'numeric', minute: '2-digit', hour12: true }),
        date: new Intl.DateTimeFormat('ar-EG', { day: 'numeric', month: 'long' }),
        fullDate: new Intl.DateTimeFormat('ar-EG', { day: 'numeric', month: 'long', year: 'numeric' })
    };

    function formatTime(dateStr) {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        if (isNaN(d.getTime())) return '';
        return arabicFormatter.time.format(d);
    }

    function getDateKey(dateStr) {
        const d = new Date(dateStr);
        if (isNaN(d.getTime())) return '';
        return `${d.getFullYear()}-${d.getMonth()}-${d.getDate()}`;
    }

    function formatDateLabel(dateStr) {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        if (isNaN(d.getTime())) return '';
        
        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);
        
        const targetDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        
        if (targetDate.getTime() === today.getTime()) return 'اليوم';
        if (targetDate.getTime() === yesterday.getTime()) return 'أمس';
        
        if (d.getFullYear() !== now.getFullYear()) {
            return arabicFormatter.fullDate.format(d);
        }
        return arabicFormatter.date.format(d);
    }

    function formatInboxTime(dateStr) {
        if (!dateStr) return '';
        const d = new Date(dateStr);
        if (isNaN(d.getTime())) return '';
        
        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        const targetDate = new Date(d.getFullYear(), d.getMonth(), d.getDate());
        
        if (targetDate.getTime() === today.getTime()) {
            return formatTime(dateStr);
        }
        
        const yesterday = new Date(today);
        yesterday.setDate(yesterday.getDate() - 1);
        if (targetDate.getTime() === yesterday.getTime()) return 'أمس';
        
        if (d.getFullYear() !== now.getFullYear()) {
            return d.toLocaleDateString('ar-EG', { day: 'numeric', month: 'numeric', year: '2-digit' });
        }
        return arabicFormatter.date.format(d);
    }

    // Persist state across navigations
    const STATE_KEY = 'wessal_chat_state';

    function saveState() {
        sessionStorage.setItem(STATE_KEY, JSON.stringify({
            isOpen,
            currentView,
            activeThreadUserId,
            timestamp: Date.now()
        }));
    }

    function loadState() {
        const saved = sessionStorage.getItem(STATE_KEY);
        if (saved) {
            try {
                const parsed = JSON.parse(saved);
                // Check 30 minute expiration (30 * 60 * 1000 ms = 1800000 ms)
                if (parsed.timestamp && (Date.now() - parsed.timestamp) < 1800000) {
                    return parsed;
                } else {
                    sessionStorage.removeItem(STATE_KEY);
                }
            } catch(e) {}
        }
        return null;
    }

    async function init(userId) {
        currentUserId = userId;
        container = document.getElementById('w-floating-chat-container');
        if (!container) return; // Not logged in or widget missing

        btn = document.getElementById('w-floating-chat-btn');
        windowEl = document.getElementById('w-chat-window');
        badge = document.getElementById('w-floating-chat-badge');
        
        inboxView = document.getElementById('w-chat-inbox-view');
        threadView = document.getElementById('w-chat-thread-view');
        
        inboxList = document.getElementById('w-chat-inbox-list');
        messagesArea = document.getElementById('w-chat-messages');
        form = document.getElementById('w-chat-form');
        input = document.getElementById('w-chat-input');
        threadName = document.getElementById('w-chat-thread-name');
        threadContext = document.getElementById('w-chat-thread-context');
        backBtn = document.getElementById('w-chat-back-btn');
        backBadge = document.getElementById('w-chat-back-badge');
        emptyTemplate = document.getElementById('w-chat-empty-template');

        // Bind events
        btn.addEventListener('click', toggleChat);
        backBtn.addEventListener('click', showInbox);
        form.addEventListener('submit', sendMessage);
        
        // Close buttons inside the widget
        document.querySelectorAll('.w-chat-close-btn').forEach(btn => {
            btn.addEventListener('click', closeChat);
        });

        // Initialize SignalR
        connection = new signalR.HubConnectionBuilder()
            .withUrl('/hubs/chat')
            .withAutomaticReconnect()
            .build();

        connection.on('ReceiveGlobalMessage', handleIncomingMessage);

        try {
            await connection.start();
        } catch (e) {
            console.error('Chat SignalR connection error:', e);
        }

        // Fetch initial unread count
        await fetchUnreadCount();

        // Restore state
        const state = loadState();
        if (state && state.isOpen) {
            openChat();
            if (state.currentView === 'thread' && state.activeThreadUserId) {
                // Determine name from recent conversations or default
                openThread(state.activeThreadUserId, 'مستخدم', ''); 
            } else {
                showInbox();
            }
        }
    }

    function toggleChat() {
        if (isOpen) closeChat();
        else openChat();
    }

    function openChat() {
        isOpen = true;
        windowEl.classList.remove('d-none');
        btn.classList.add('w-chat-open');
        if (currentView === 'inbox') {
            showInbox();
        }
        saveState();
    }

    function closeChat() {
        isOpen = false;
        windowEl.classList.add('d-none');
        btn.classList.remove('w-chat-open');
        saveState();
    }

    async function showInbox() {
        currentView = 'inbox';
        activeThreadUserId = null;
        
        inboxView.classList.add('active');
        inboxView.classList.remove('slide-out-left', 'slide-out-right');
        
        threadView.classList.remove('active');
        threadView.classList.add('slide-out-right');
        
        backBadge.classList.add('d-none');
        saveState();

        if (hasLoadedInbox) {
            // Remove active classes
            document.querySelectorAll('.w-chat-inbox-row').forEach(row => {
                row.classList.remove('is-active');
            });
            return;
        }

        // Load recent conversations
        inboxList.innerHTML = '<div class="w-chat-loading-state">جاري التحميل...</div>';
        try {
            const res = await fetch('/Chat/GetRecentConversations');
            if (!res.ok) throw new Error('فشل في جلب المحادثات الأخيرة');
            const data = await res.json();
            renderInbox(data);
            hasLoadedInbox = true;
        } catch (e) {
            inboxList.innerHTML = '<div class="w-chat-loading-state" style="color:var(--w-danger)">حدث خطأ أثناء تحميل الرسائل.</div>';
        }
    }

    function renderInbox(data) {
        if (!data || data.length === 0) {
            inboxList.innerHTML = emptyTemplate.innerHTML;
            return;
        }

        inboxList.innerHTML = '';
        data.forEach(row => {
            const el = document.createElement('div');
            el.className = `w-chat-inbox-row ${row.unreadCount > 0 ? 'unread' : ''} ${String(row.otherUserId) === String(activeThreadUserId) ? 'is-active' : ''}`;
            el.dataset.userId = row.otherUserId;
            el.onclick = () => openThread(row.otherUserId, row.otherUserName, '');
            
            const initial = (row.otherUserName || 'U').substring(0, 1).toUpperCase();
            
            el.innerHTML = `
                <div class="w-chat-avatar">${initial}</div>
                <div class="w-chat-inbox-content">
                    <div class="w-chat-inbox-top">
                        <div class="w-chat-inbox-name">${row.otherUserName}</div>
                        <div class="w-chat-inbox-time">
                            ${formatInboxTime(row.lastMessageAt)}
                        </div>
                    </div>
                    <div class="w-chat-inbox-bottom">
                        <div class="w-chat-preview">${row.lastMessagePreview}</div>
                        ${row.unreadCount > 0 ? `<div class="w-chat-unread-dot"></div>` : ''}
                    </div>
                </div>
            `;
            inboxList.appendChild(el);
        });
    }

    async function openThread(userId, userName, contextType = '', contextId = '') {
        // Fallbacks if userName is missing (e.g. restoring state)
        if (!userName) userName = 'مستخدم';
        
        currentView = 'thread';
        activeThreadUserId = userId;
        isOpen = true;

        threadName.textContent = userName;
        threadContext.textContent = contextType ? `${contextType} ${contextId ? '#' + contextId : ''}`.trim() : 'متصل الآن';
        
        // Setup Avatar
        const avatarEl = document.getElementById('w-chat-thread-avatar');
        if (avatarEl) {
            avatarEl.textContent = userName.substring(0, 1).toUpperCase();
        }

        // Update active class in inbox visually in case it's in DOM
        document.querySelectorAll('.w-chat-inbox-row').forEach(row => {
            if (row.dataset.userId == userId) {
                row.classList.add('is-active');
            } else {
                row.classList.remove('is-active');
            }
        });
        
        windowEl.classList.remove('d-none');
        btn.classList.add('w-chat-open');

        inboxView.classList.remove('active');
        inboxView.classList.add('slide-out-right'); // slides to right when entering thread
        
        threadView.classList.add('active');
        threadView.classList.remove('slide-out-right', 'slide-out-left');

        messagesArea.innerHTML = '<div class="w-chat-loading-state">جاري التحميل...</div>';
        saveState();

        try {
            await fetch(`/Chat/MarkAsRead?otherUserId=${userId}`, { method: 'POST' });
            fetchUnreadCount(); // update badge
            
            if (connection.state === signalR.HubConnectionState.Connected) {
                await connection.invoke('JoinThread', userId);
            }

            const res = await fetch(`/Chat/History?otherUserId=${userId}`);
            if (res.ok) {
                const data = await res.json();
                renderMessages(data);
            }
        } catch (e) {
            messagesArea.innerHTML = '<div class="w-chat-loading-state" style="color:var(--w-danger)">حدث خطأ.</div>';
        }
    }

    function renderMessages(messages) {
        messagesArea.innerHTML = '';
        lastRenderedDate = null; // Reset track

        if (!messages || messages.length === 0) {
            messagesArea.innerHTML = '<div class="w-chat-loading-state">ابدأ المحادثة الآن...</div>';
            return;
        }

        messages.forEach(msg => {
            appendMessageUI(msg, false); // Don't scroll yet
        });
        scrollToBottom(true); // force scroll on full load
    }

    function appendMessageUI(msg, autoScroll = true) {
        const isSent = String(msg.senderId) === String(currentUserId);
        
        // Handle Date Separator
        const msgDateKey = getDateKey(msg.sentAt);
        if (msgDateKey !== lastRenderedDate) {
            const separator = document.createElement('div');
            separator.className = 'w-chat-date-separator';
            separator.innerHTML = `<span class="w-chat-date-text">${formatDateLabel(msg.sentAt)}</span>`;
            messagesArea.appendChild(separator);
            lastRenderedDate = msgDateKey;
        }

        const wrap = document.createElement('div');
        wrap.className = `w-chat-message-row ${isSent ? 'is-sent' : 'is-received'}`;
        
        // Grouping logic
        const lastRow = messagesArea.lastElementChild;
        // Ensure we don't group with the separator
        if (lastRow && lastRow.classList.contains('w-chat-message-row')) {
            const wasSent = lastRow.classList.contains('is-sent');
            
            if (isSent === wasSent) {
                // Same sender
                wrap.classList.add('group-last');
                
                if (lastRow.classList.contains('group-standalone')) {
                    lastRow.classList.remove('group-standalone');
                    lastRow.classList.add('group-first');
                } else if (lastRow.classList.contains('group-last')) {
                    lastRow.classList.remove('group-last');
                    lastRow.classList.add('group-middle');
                }
            } else {
                // Different sender
                wrap.classList.add('group-standalone');
            }
        } else {
            // First message in this day block or overall
            wrap.classList.add('group-standalone');
        }

        wrap.innerHTML = `
            <div class="w-chat-bubble ${isSent ? 'sent' : 'received'}">
                <div class="w-chat-bubble-text">${msg.message}</div>
                <div class="w-chat-bubble-time">
                    ${formatTime(msg.sentAt)}
                </div>
            </div>
        `;
        
        // Remove empty state if present
        if (messagesArea.querySelector('.w-chat-loading-state')) {
            messagesArea.innerHTML = '';
        }
        
        messagesArea.appendChild(wrap);
        if (autoScroll) scrollToBottom();
    }

    function scrollToBottom(force = false) {
        if (force) {
            messagesArea.scrollTop = messagesArea.scrollHeight;
            return;
        }
        
        // Scroll Anchoring: Only scroll if we are within 100px of the bottom
        const threshold = 100;
        const isNearBottom = messagesArea.scrollHeight - messagesArea.scrollTop - messagesArea.clientHeight <= threshold;
        
        if (isNearBottom) {
            messagesArea.scrollTop = messagesArea.scrollHeight;
        }
    }

    async function sendMessage(e) {
        e.preventDefault();
        const text = input.value.trim();
        if (!text || !activeThreadUserId) return;

        input.value = '';
        input.focus();

        try {
            hasLoadedInbox = false; // Invalidate inbox cache
            await connection.invoke('SendMessage', activeThreadUserId, text);
        } catch (e) {
            console.error('Send error:', e);
        }
    }

    function handleIncomingMessage(payload) {
        // Re-fetch global unread count
        fetchUnreadCount();

        if (isOpen) {
            if (currentView === 'inbox') {
                hasLoadedInbox = false; // force reload next time, or just reload now
                showInbox(); // refresh list
            } else if (currentView === 'thread') {
                hasLoadedInbox = false; // invalidate cache
                if (String(payload.senderId) === String(activeThreadUserId) || String(payload.receiverId) === String(activeThreadUserId)) {
                    appendMessageUI(payload);
                    if (String(payload.senderId) === String(activeThreadUserId)) {
                        fetch(`/Chat/MarkAsRead?otherUserId=${activeThreadUserId}`, { method: 'POST' });
                        fetchUnreadCount();
                    }
                } else if (String(payload.senderId) !== String(currentUserId)) {
                    // Message from someone else while in a thread
                    backBadge.classList.remove('d-none');
                }
            }
        } else {
            // Closed widget
            if (String(payload.senderId) !== String(currentUserId)) {
                // One-time soft 3-second glow
                btn.animate([
                    { boxShadow: '0 4px 15px rgba(99, 102, 241, 0.2)' },
                    { boxShadow: '0 0 25px 5px rgba(99, 102, 241, 0.6)' },
                    { boxShadow: '0 4px 15px rgba(99, 102, 241, 0.2)' }
                ], { duration: 3000, easing: 'ease-out' });
            }
        }
    }

    async function fetchUnreadCount() {
        try {
            const res = await fetch('/Chat/GetUnreadCount');
            if (res.ok) {
                const data = await res.json();
                updateBadge(data.unreadCount);
            }
        } catch(e) {}
    }

    function updateBadge(count) {
        unreadCount = count;
        if (count > 0) {
            badge.textContent = count > 99 ? '+99' : count;
            badge.classList.remove('d-none');
        } else {
            badge.classList.add('d-none');
        }
    }

    return {
        init: init,
        openThread: openThread,
        toggle: toggleChat
    };
})();
