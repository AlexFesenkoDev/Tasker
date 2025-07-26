-- Create log table in DB
-- Run only one time

CREATE TABLE Logs (
    Id SERIAL PRIMARY KEY,
    Timestamp TIMESTAMPTZ NOT NULL,
    Level VARCHAR(50),
    Message TEXT,
    MessageTemplate TEXT,
    Exception TEXT,
    Properties JSONB
);