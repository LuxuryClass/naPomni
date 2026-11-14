// src/components/layout/Sidebar.tsx
import { Settings } from '@skbkontur/react-icons';
import { Tooltip } from '@skbkontur/react-ui';
import type { Team, UserProfile } from '../../../types';
import logo from '../../../../public/logo.png';
import s from './Sidebar.module.css';

interface Props {
  expanded: boolean;
  onToggle: () => void;
  teams: Team[];
  activeTeamId: string | null;
  onSelectTeam: (id: string | null) => void;
  user: UserProfile;
  onProfile: () => void;
}

function TeamBadge({ team, size }: { team: Team; size: number }) {
  return (
    <span className={s.badge} style={{ background: team.color, width: size, height: size, fontSize: size * 0.38 }}>
      {team.short}
    </span>
  );
}

export function Sidebar({ expanded, onToggle, teams, activeTeamId, onSelectTeam, user, onProfile }: Props) {
  const topTeams = teams.slice(0, 5);
  const select = (id: string) => onSelectTeam(activeTeamId === id ? null : id);

  return (
    <aside className={`${s.sidebar}${expanded ? ` ${s.expanded}` : ''}`}>
      <button className={s.appBtn} onClick={onToggle} title={expanded ? 'Свернуть панель' : 'Развернуть панель'}>
        <span className={s.logo}>
          <img src={logo} alt="КонтурНаПомни" />
        </span>
        <span className={`${s.appName} ${s.fadeText}`}>КонтурНаПомни</span>
      </button>

      <div className={`${s.caption} ${s.fadeText}`}>Команды</div>

      <nav className={s.teams}>
        {topTeams.map(team => {
          const active = activeTeamId === team.id;
          const btn = (
            <button
              className={`${s.teamBtn}${active ? ` ${s.active}` : ''}`}
              onClick={() => select(team.id)}
            >
              <TeamBadge team={team} size={36} />
              <span className={`${s.teamName} ${s.fadeText}`}>{team.name}</span>
            </button>
          );

          return expanded ? (
            <div key={team.id} className={s.teamWrap}>{btn}</div>
          ) : (
            <Tooltip key={team.id} pos="right middle" render={() => team.name}>
              {btn}
            </Tooltip>
          );
        })}
      </nav>

      <div className={s.spacer} />

      <button className={s.row} onClick={onProfile} title="Настройки профиля">
        <span className={s.iconSlot}>
          <Settings size={18} />
        </span>
        <span className={`${s.rowLabel} ${s.fadeText}`}>Настройки профиля</span>
      </button>

      <button className={s.userBtn} onClick={onProfile} title={user.name}>
        <span className={s.avatar}>{user.id}</span>
        <span className={`${s.userMeta} ${s.fadeText}`}>
          <b>{user.name}</b>
          <small>{user.email}</small>
        </span>
      </button>
    </aside>
  );
}
