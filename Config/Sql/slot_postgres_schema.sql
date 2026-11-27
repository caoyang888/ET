-- Slot 服务端 PostgreSQL 结构。脚本可重复执行。

CREATE TABLE IF NOT EXISTS slot_account (
    account TEXT PRIMARY KEY,
    player_id BIGINT NOT NULL,
    nickname TEXT NOT NULL,
    avatar TEXT NOT NULL DEFAULT '',
    wallet_json JSONB NOT NULL,
    vip_exp INTEGER NOT NULL DEFAULT 0,
    real_name_verified BOOLEAN NOT NULL DEFAULT FALSE,
    phone_bound BOOLEAN NOT NULL DEFAULT FALSE,
    created_unix BIGINT NOT NULL,
    updated_unix BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS slot_progress (
    account TEXT NOT NULL,
    progress_group INTEGER NOT NULL,
    stage_id INTEGER NOT NULL,
    boss_hp BIGINT NOT NULL,
    cleared BOOLEAN NOT NULL DEFAULT FALSE,
    free_spins INTEGER NOT NULL DEFAULT 0,
    locked_stake_id INTEGER NOT NULL DEFAULT 0,
    locked_line_count INTEGER NOT NULL DEFAULT 0,
    miss_streak INTEGER NOT NULL DEFAULT 0,
    pity_cooldown INTEGER NOT NULL DEFAULT 0,
    total_bet BIGINT NOT NULL DEFAULT 0,
    total_win BIGINT NOT NULL DEFAULT 0,
    total_damage BIGINT NOT NULL DEFAULT 0,
    recharge_cents INTEGER NOT NULL DEFAULT 0,
    total_cost BIGINT NOT NULL DEFAULT 0,
    clear_count INTEGER NOT NULL DEFAULT 0,
    PRIMARY KEY (account, progress_group)
);

CREATE TABLE IF NOT EXISTS slot_room_ledger (
    room_id INTEGER PRIMARY KEY,
    public_stock BIGINT NOT NULL,
    rank_pool BIGINT NOT NULL,
    jackpot_pool BIGINT NOT NULL,
    version BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS slot_spin (
    account TEXT NOT NULL,
    client_spin_id TEXT NOT NULL,
    player_id BIGINT NOT NULL,
    room_id INTEGER NOT NULL,
    stake_id INTEGER NOT NULL,
    bet BIGINT NOT NULL,
    win BIGINT NOT NULL,
    damage BIGINT NOT NULL,
    seed BIGINT NOT NULL,
    result_json JSONB NOT NULL,
    created_unix BIGINT NOT NULL,
    PRIMARY KEY (account, client_spin_id)
);

CREATE TABLE IF NOT EXISTS slot_ops (
    account TEXT PRIMARY KEY,
    jubao_level INTEGER NOT NULL,
    jubao_ready_unix BIGINT NOT NULL,
    dice_cell INTEGER NOT NULL,
    dice_laps INTEGER NOT NULL,
    piggy BIGINT NOT NULL,
    bankruptcy_claims INTEGER NOT NULL,
    task_json JSONB NOT NULL,
    claim_json JSONB NOT NULL,
    king_json JSONB NOT NULL,
    lap_json JSONB NOT NULL,
    redeem_json JSONB NOT NULL,
    order_json JSONB NOT NULL
);

CREATE TABLE IF NOT EXISTS slot_order (
    order_id TEXT PRIMARY KEY,
    account TEXT NOT NULL,
    player_id BIGINT NOT NULL,
    product_id INTEGER NOT NULL,
    amount BIGINT NOT NULL,
    status TEXT NOT NULL,
    receipt TEXT NOT NULL DEFAULT '',
    created_unix BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS slot_mail (
    mail_id BIGSERIAL PRIMARY KEY,
    account TEXT NOT NULL,
    player_id BIGINT NOT NULL,
    title TEXT NOT NULL,
    body TEXT NOT NULL,
    attachment_json JSONB NOT NULL,
    is_read BOOLEAN NOT NULL DEFAULT FALSE,
    is_claimed BOOLEAN NOT NULL DEFAULT FALSE,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    expire_unix BIGINT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS slot_rank (
    account TEXT NOT NULL,
    period INTEGER NOT NULL,
    player_id BIGINT NOT NULL,
    nickname TEXT NOT NULL,
    damage BIGINT NOT NULL,
    PRIMARY KEY (account, period)
);

CREATE TABLE IF NOT EXISTS slot_redeem_code (
    code TEXT PRIMARY KEY,
    item_id INTEGER NOT NULL,
    count BIGINT NOT NULL
);

CREATE TABLE IF NOT EXISTS slot_maintenance (
    id INTEGER PRIMARY KEY,
    enabled BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS slot_king_quota (
    tier_id INTEGER PRIMARY KEY,
    taken INTEGER NOT NULL
);

INSERT INTO slot_maintenance (id, enabled) VALUES (1, FALSE) ON CONFLICT (id) DO NOTHING;
