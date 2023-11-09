import { Avatar, Button } from "antd";
import {
  UserOutlined,
  PhoneOutlined,
  VideoCameraOutlined,
  InfoCircleOutlined,
} from "@ant-design/icons";

import styles from "./css/BoxChat.module.css";

/**
 * @param {*} props
 *      @param: avatar
 *      @param: name
 *      @param: time
 * @returns 
 */
const HeaderBoxChat = (props) => {
  return (
    <div className={styles["header-box-chart"]}>
      <Button className={styles["btn-avatar"]}>
        <Avatar size="large" icon={<UserOutlined />} />
        <div>
          <p>{props.name}</p>
          <p className={styles["time-active"]}>{props.time}</p>
        </div>
      </Button>
      <div>
        <Button className="mg-3" icon={<PhoneOutlined />} size="large"></Button>
        <Button
          className="mg-3"
          icon={<VideoCameraOutlined />}
          size="large"
        ></Button>
        <Button
          className="mg-3"
          icon={<InfoCircleOutlined />}
          size="large"
        ></Button>
      </div>
    </div>
  );
};

export default HeaderBoxChat;
