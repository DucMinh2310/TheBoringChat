import { Avatar, Button } from "antd";
import {
  UserOutlined,
  PhoneOutlined,
  VideoCameraOutlined,
  InfoCircleOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import styles from "./css/ItemChat.module.css";

/**
 *
 * @param {*} props
 *    @param avatar
 *    @param name
 *    @param time
 *    @param mess
 * @returns
 */
const ItemChat = (props) => {
  return (
    <div className={styles["header-box-chart"]}>
      <Avatar size="large" icon={<UserOutlined />} />
      <div>
        <div className="flex-space-between">
          <div className={styles["div-avatar"]}>
            <div>
              <p>{props.name}</p>
            </div>
          </div>
          <div>
            <p className={styles["time-active"]}>{props.time}</p>
          </div>
        </div>
        <div className={styles["div-mess"]}>
          <p>{props.mess}</p>
        </div>
      </div>
    </div>
  );
};

export default ItemChat;
