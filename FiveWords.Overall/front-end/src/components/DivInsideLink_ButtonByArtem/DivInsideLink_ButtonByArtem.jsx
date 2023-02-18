import styles from './DivInsideLink_ButtonByArtem.module';
import classnames from 'classnames';

const DivInsideLink_ButtonByArtem = ({ label, width, handleClick, linkClassNames, innerDivClassNames }) => {
    return (
        <a
            className={classnames(styles.link, linkClassNames)}
            href="#"
            onClick={handleClick}
        >
            <div style={{ width }} className={classnames(styles.innerDiv, innerDivClassNames)}>
                {label}
            </div>
        </a>
    );
}

export default DivInsideLink_ButtonByArtem;